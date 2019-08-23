using System.Linq;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading.Tasks;
using FinTech.Domain;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;
using System.Net.Http;
using FinTech.ApplicationServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace FinTech.API.Wechat.Jobs
{
    [DisallowConcurrentExecution]
    public class TimedReminderJob : AbstractJob
    {

        public TimedReminderJob(ILogger<TimedReminderJob> logger, IConfiguration configuration, IServiceProvider provider) : base(logger)
        {
            _provider = provider;
            Url = configuration["Wechat:AccessTokenUrl"];
            Appid = configuration["Wechat:Appid"];
            Secret = configuration["Wechat:Secret"];
        }
        protected string Url { get; }
        protected string Appid { get; }
        protected string Secret { get; }
        private readonly IServiceProvider _provider;

        public override async Task Execute()
        {
            using (var scope = _provider.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<ITimedReminderService>();
                await service.TimedReminder(Url, Appid, Secret);
            }
        }


    }
}