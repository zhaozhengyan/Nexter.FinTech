using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;

namespace Nexter.Fintech.EntityFramework.Core;

[AppDbContext("Fintech", DbProvider.MySql)]
public class DefaultDbContext : AppDbContext<DefaultDbContext>
{
    public DefaultDbContext(DbContextOptions<DefaultDbContext> options) : base(options)
    {
    }
}
