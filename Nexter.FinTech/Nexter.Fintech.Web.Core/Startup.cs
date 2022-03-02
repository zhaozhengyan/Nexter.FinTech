using FinTech.API.Wechat.Infrastructure;
using FinTech.ApplicationServices.WeChat;
using Furion;
using Furion.DependencyInjection;
using Furion.Logging.Extensions;
using Furion.TaskScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexter.Fintech.Application.Job;
using Nexter.Fintech.Application.Wechat.Options;
using Serilog;
using System;
using Yitter.IdGenerator;

namespace Nexter.Fintech.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfigurableOptions<WechatOptions>();
        var wechat = App.GetConfig<WechatOptions>("Wechat", true);
        services.AddHttpApi<IWechatApi>(o =>
        {
            o.HttpHost = new Uri(wechat.AuthUrl);
        });

        YitIdHelper.SetIdGenerator(new IdGeneratorOptions(1) { SeqBitLength = 12 });

        services.AddCorsAccessor();
        services.AddTaskScheduler();

        services.AddControllers()
                .AddInjectWithUnifyResult();

        services.AddMvcFilter<SessionFilter>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            AddJobs();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseSerilogRequestLogging();

        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    private static void AddJobs()
    {
        SpareTime.Do("*/1 * * * *", (timer, count) =>
        {
            Scoped.Create((_, scope) =>
            {
                var services = scope.ServiceProvider;
                var taskService = services.GetService<ITimedReminderJob>();
                $"Job Begin[{count}]-----------------------".LogInformation<Startup>();
                taskService.Do().Wait();
                $"Job End[{count}]-------------------------".LogInformation<Startup>();
            });
        }, "SyncSequence", "定时同步MES工序");
    }
}
