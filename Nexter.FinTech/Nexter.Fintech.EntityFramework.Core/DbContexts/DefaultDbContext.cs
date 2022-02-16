using FinTech.Domain;
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using Yitter.IdGenerator;

namespace Nexter.Fintech.EntityFramework.Core;

[AppDbContext("Fintech", DbProvider.MySql)]
public class DefaultDbContext : AppDbContext<DefaultDbContext>
{
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }

    protected override void SavingChangesEvent(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;

        var entities = dbContext.ChangeTracker.Entries().Where(u => u.State == EntityState.Added || u.State == EntityState.Modified || u.State == EntityState.Deleted);

        foreach (var entity in entities)
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Property(nameof(BaseEntity.Id)).CurrentValue = YitIdHelper.NextId();
                    break;
            }
        }
    }
}
