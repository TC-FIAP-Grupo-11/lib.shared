using Microsoft.EntityFrameworkCore;
using FCG.Lib.Shared.Domain.Entities;

namespace FCG.Lib.Shared.Infrastructure.Data.Extensions;

public static class DbContextExtensions
{
    public static async Task<int> SaveChangesWithAuditAsync(
        this DbContext context,
        CancellationToken cancellationToken = default)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }

            entity.UpdatedAt = DateTime.UtcNow;
        }

        return await context.SaveChangesAsync(cancellationToken);
    }
}
