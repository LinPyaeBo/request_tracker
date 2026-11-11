using Microsoft.EntityFrameworkCore;
using RequestTracker.Api.Data;
using RequestTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Host=localhost;Port=5432;Database=request_tracker;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

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
    var errors = Validate(input.Title, input.Description, input.RequestedBy);
    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var request = new ServiceRequest
    {
        Title = input.Title.Trim(),
        Description = input.Description.Trim(),
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

static Dictionary<string, string[]> Validate(string? title, string? description, string? requestedBy)
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(title))
        errors["title"] = ["A title is required."];
    else if (title.Trim().Length > 120)
        errors["title"] = ["The title must be 120 characters or fewer."];

    if (string.IsNullOrWhiteSpace(description))
        errors["description"] = ["A description is required."];
    else if (description.Trim().Length > 2000)
        errors["description"] = ["The description must be 2000 characters or fewer."];

    if (requestedBy?.Trim().Length > 120)
        errors["requestedBy"] = ["Requested by must be 120 characters or fewer."];

    return errors;
}

public sealed record CreateRequestInput(string? Title, string? Description, string? RequestedBy);
public sealed record UpdateStatusInput(string? Status);

public partial class Program;
