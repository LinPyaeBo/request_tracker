using Microsoft.EntityFrameworkCore;
using RequestTracker.Api.Models;

namespace RequestTracker.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var request = modelBuilder.Entity<ServiceRequest>();
        request.ToTable("service_requests");
        request.HasKey(x => x.Id);
        request.Property(x => x.Title).HasMaxLength(120).IsRequired();
        request.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        request.Property(x => x.RequestedBy).HasMaxLength(120);
        request.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        request.Property(x => x.CreatedAtUtc).IsRequired();
    }
}
