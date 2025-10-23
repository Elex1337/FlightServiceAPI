using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace FlightServiceAPI.Infrastructure.Data;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly ILogger<AuditableEntityInterceptor> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditableEntityInterceptor(
        ILogger<AuditableEntityInterceptor> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        LogChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        LogChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void LogChanges(DbContext? context)
    {
        if (context == null) return;

        var username = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value 
                       ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value
                       ?? "System";

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || 
                       e.State == EntityState.Modified || 
                       e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            var entityName = entry.Entity.GetType().Name;
            var state = entry.State.ToString();
            var timestamp = DateTime.UtcNow;

            _logger.LogInformation(
                "Database change by User={Username}: Entity={EntityName}, State={State}, Timestamp={Timestamp}, Changes={Changes}",
                username,
                entityName,
                state,
                timestamp,
                GetChangedProperties(entry));
        }
    }

    private static string GetChangedProperties(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        var changes = new List<string>();

        foreach (var property in entry.Properties)
        {
            if (entry.State == EntityState.Added)
            {
                changes.Add($"{property.Metadata.Name}={property.CurrentValue}");
            }
            else if (entry.State == EntityState.Modified && property.IsModified)
            {
                changes.Add($"{property.Metadata.Name}: {property.OriginalValue} -> {property.CurrentValue}");
            }
            else if (entry.State == EntityState.Deleted)
            {
                changes.Add($"{property.Metadata.Name}={property.OriginalValue}");
            }
        }

        return string.Join(", ", changes);
    }
}