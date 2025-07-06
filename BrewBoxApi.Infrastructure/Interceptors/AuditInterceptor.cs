using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BrewBoxApi.Domain.SeedWork;
using BrewBoxApi.Infrastructure.Identity;

namespace BrewBoxApi.Infrastructure.Interceptors;

public class AuditInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseModel && e.State == EntityState.Added);

        foreach (var entry in entries)
        {
            var entity = (BaseModel)entry.Entity;
            entity.CreatedOn = DateTime.UtcNow;
            entity.CreatedById = currentUserService.UserId ?? "System";
        }
    }
}