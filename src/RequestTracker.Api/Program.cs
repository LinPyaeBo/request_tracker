using Microsoft.EntityFrameworkCore;
using RequestTracker.Api;
using RequestTracker.Api.Data;
using RequestTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Set the ConnectionStrings__Default environment variable.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/requests", async (AppDbContext db, CancellationToken ct) =>
    Results.Ok(await db.ServiceRequests
        .AsNoTracking()
        .OrderByDescending(x => x.CreatedAtUtc)
        .ToListAsync(ct)));

app.MapGet("/api/requests/{id:int}", async (int id, AppDbContext db, CancellationToken ct) =>
{
    var request = await db.ServiceRequests.AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id, ct);
    return request is null ? Results.NotFound() : Results.Ok(request);
});

app.MapPost("/api/requests", async (CreateRequestInput input, AppDbContext db, CancellationToken ct) =>
{
    var errors = RequestInputValidator.Validate(input.Title, input.Description, input.RequestedBy);
    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var request = new ServiceRequest
    {
        Title = input.Title!.Trim(),
        Description = input.Description!.Trim(),
        RequestedBy = string.IsNullOrWhiteSpace(input.RequestedBy) ? null : input.RequestedBy.Trim()
    };

    db.ServiceRequests.Add(request);
    await db.SaveChangesAsync(ct);
    return Results.Created($"/api/requests/{request.Id}", request);
});

app.MapPut("/api/requests/{id:int}/status", async (int id, UpdateStatusInput input, AppDbContext db, CancellationToken ct) =>
{
    if (!Enum.TryParse<RequestStatus>(input.Status, true, out var status))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["status"] = ["Status must be Pending, InProgress, or Completed."]
        });

    var request = await db.ServiceRequests.FindAsync([id], ct);
    if (request is null)
        return Results.NotFound();

    request.Status = status;
    request.UpdatedAtUtc = DateTime.UtcNow;
    await db.SaveChangesAsync(ct);
    return Results.Ok(request);
});

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public sealed record CreateRequestInput(string? Title, string? Description, string? RequestedBy);
public sealed record UpdateStatusInput(string? Status);

public partial class Program { }
