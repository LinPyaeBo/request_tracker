namespace RequestTracker.Api.Models;

public enum RequestStatus
{
    Pending,
    InProgress,
    Completed
}

public sealed class ServiceRequest
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? RequestedBy { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
}
