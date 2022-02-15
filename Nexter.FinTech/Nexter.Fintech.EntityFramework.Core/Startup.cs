using Furion;
using Microsoft.Extensions.DependencyInjection;

namespace Nexter.Fintech.EntityFramework.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseAccessor(options =>
        {
            options.AddDbPool<DefaultDbContext>();
        }, "Nexter.Fintech.Database.Migrations");
    }
}
