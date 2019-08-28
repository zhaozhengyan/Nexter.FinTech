using Xunit.Abstractions;
using FinTech.API.Wechat.Infrastructure;
using FinTech.ApplicationServices;
using FinTech.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nexter.Domain;
using Nexter.Infrastructure;
using Refit;
using System;

namespace FinTech.Test
{
    public class BaseTest
    {
        protected virtual string EnvName => "Development";
        protected readonly ITestOutputHelper Output;
        public readonly IServiceProvider ServiceProvider;
        public readonly IConfiguration Configuration;

        public BaseTest(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
            var services = new ServiceCollection();
            services
                .AddTransient<WechatApiHttpHandler>()
                .AddRefitClient<IWeChatApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(ConfigurationExtension.Configuration["Wechat:BaseUrl"]))
                .AddHttpMessageHandler<WechatApiHttpHandler>();
            services
                .AddLogging()
                .AddConfiguration(Configuration, EnvName)
                .AddScoped<Repository>()
                .AddScoped<DbContext, NexterContext>(sp => sp.GetRequiredService<NexterContext>())
                .AddScoped<IUnitOfWork, Repository>(sp => sp.GetRequiredService<Repository>())
                .AddScoped<IRepository, Repository>(sp => sp.GetRequiredService<Repository>());
            services.AddDbContextPool<NexterContext>(opt =>
            {
                opt.UseSqlServer(ConfigurationExtension.Configuration.GetConnectionString("Writable"));
                opt.EnableSensitiveDataLogging();
            });
            services.AddScoped<ITimedReminderService, TimedReminderService>();
            Configuration = ConfigurationExtension.Configuration;
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}