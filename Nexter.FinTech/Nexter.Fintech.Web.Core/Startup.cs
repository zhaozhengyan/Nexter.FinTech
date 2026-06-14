using FinTech.API.Wechat.Infrastructure;
using FinTech.ApplicationServices.WeChat;
using Furion;
using Furion.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nexter.Fintech.Application.Job;
using Nexter.Fintech.Application.Wechat.Options;
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

        //TODO id long dto transcation to string
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions(1));

        services.AddCorsAccessor();

        services.AddHostedService<TimedReminderBackgroundService>();

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

        app.UseHttpsRedirection();

        app.UseStaticFiles();

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

}
