using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FinTech.API.Wechat.Jobs
{
    [DisallowConcurrentExecution]
    public abstract class AbstractJob : IJob
    {
        public IServiceScope ServiceScope { get; internal set; }
        protected IJobExecutionContext Context { get; set; }
        protected readonly ILogger Logger;

        protected AbstractJob(ILogger<AbstractJob> logger)
        {
            Logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            this.Context = context;
            try
            {
                var timer = new Stopwatch();
                timer.Start();
                Logger.LogInformation($"job {context.JobDetail.Key} fired by trigger {context.Trigger.Key}");
                await Execute();
                timer.Stop();
                Logger.LogInformation($"job {context.JobDetail.Key} execution complete, during {timer.ElapsedMilliseconds}ms", timer.ElapsedMilliseconds);
                context.Result = "Success";
            }
            catch (Exception ex)
            {
                context.Result = "Error";
                Logger.LogError(ex.Message, ex);
            }
        }
        public abstract Task Execute();
    }
}