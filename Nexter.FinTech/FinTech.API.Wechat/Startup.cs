using FinTech.API.Wechat.Infrastructure;
using FinTech.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Nexter.Domain;
using Nexter.Infrastructure;
using Serilog;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using FinTech.API.Wechat.Jobs;
using FinTech.ApplicationServices;

namespace FinTech.API.Wechat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ITimedReminderService, TimedReminderService>();
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<TimedReminderJob>();
            services.AddSingleton(new JobSchedule(typeof(TimedReminderJob), "0/50 * * * * ?")); // run every 5 seconds
            services.AddHostedService<QuartzHostedService>();
            services.AddDbContextPool<NexterContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("Writable"));
                opt.EnableSensitiveDataLogging();
            });
            services.AddMvc(options =>
                {
                    options.Filters.Add<SessionFilter>();
                    options.Filters.Add<ExceptionFilter>();
                    options.ValueProviderFactories.Add(new JQueryQueryStringValueProviderFactory());
                })
            .AddJsonOptions(opts =>
                {
                    opts.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
                    opts.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                });

            services
                .AddScoped<Repository>()
                .AddScoped<DbContext, NexterContext>(sp => sp.GetRequiredService<NexterContext>())
                .AddScoped<IUnitOfWork, Repository>(sp => sp.GetRequiredService<Repository>())
                .AddScoped<IRepository, Repository>(sp => sp.GetRequiredService<Repository>())
                ;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseCors(builder => builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
            app.Use(next => context =>
            {
                context.Request.EnableRewind();
                context.Response.EnableRewind();
                return next(context);
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMvc();
        }
    }
}
