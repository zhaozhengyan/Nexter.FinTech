using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace FinTech.Infrastructure
{
    public class ConsoleHostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }

    public class HostingEnvironment : Microsoft.AspNetCore.Hosting.IHostingEnvironment
    {
        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }

    public static class ConfigurationExtension
    {
        public static IConfiguration Configuration { get; private set; }
        public static Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment { get; private set; }

        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration = null, string envName = null)
        {
            if (configuration == null)
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                        .AddJsonFile("appsettings.json");

                builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                 "appsettings.json"),
                                    true,
                                    true)
                       .AddJsonFile("appsettings.json",
                                    true,
                                    true);

                if (string.IsNullOrWhiteSpace(envName))
                {
                    envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                }

                var hostingEnvironment = new HostingEnvironment
                {
                    EnvironmentName = envName,
                    ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
                    ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)
                };
                services.AddSingleton<Microsoft.AspNetCore.Hosting.IHostingEnvironment>(hostingEnvironment);
                HostingEnvironment = hostingEnvironment;
                var consoleHostingEnvironment = new ConsoleHostingEnvironment
                {
                    EnvironmentName = envName,
                    ApplicationName = AppDomain.CurrentDomain.FriendlyName,
                    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
                    ContentRootFileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory)
                };
                services.AddSingleton<IHostingEnvironment>(consoleHostingEnvironment);


                builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                                 $"appsettings.{envName}.json"),
                                    true,
                                    true)
                       .AddJsonFile($"appsettings.{envName}.json",
                                    true,
                                    true)
                       .AddEnvironmentVariables();

                configuration = builder.Build();
            }

            Configuration = configuration;
            return services.AddSingleton(configuration);
        }

        public static IServiceCollection AddCustomOptions<TOptions>(this IServiceCollection services,
                                                                    Action<TOptions> optionAction = null,
                                                                    string sectionName = null)
            where TOptions : class, new()
        {
            if (optionAction != null)
            {
                services.Configure(optionAction);
            }
            else
            {
                services.AddSingleton<IOptions<TOptions>>(provider =>
                {
                    var configuration = provider.GetService<IConfiguration>().GetSection(sectionName ?? typeof(TOptions).Name);
                    if (!configuration.Exists())
                    {
                        throw new ArgumentNullException($"{nameof(TOptions)}");
                    }

                    var options = new TOptions();
                    configuration.Bind(options);
                    return new OptionsWrapper<TOptions>(options);
                });
            }

            return services;
        }

        public static T Get<T>(this IConfiguration configuration, string key)
        {
            T appSetting = default;
            if (typeof(T).IsPrimitive)
            {
                appSetting = configuration.GetValue<T>(key);
            }
            else
            {
                var configSection = configuration.GetSection(key);
                if (configSection.Exists())
                {
                    appSetting = Activator.CreateInstance<T>();
                    configSection.Bind(appSetting);
                }
            }

            return appSetting;
        }
    }
}