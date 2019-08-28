using FinTech.API.Wechat.Infrastructure;
using FinTech.ApplicationServices;
using FinTech.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;
using Nexter.Domain;
using Nexter.Infrastructure;
using Refit;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace FinTech.Test
{
    public class TimedReminderTests : BaseTest
    {
        protected override string EnvName => "Development";
        public TimedReminderTests(ITestOutputHelper tempOutput) : base(tempOutput)
        {
        }

        [Fact]
        public async Task TestTimedReminder()
        {
            try
            {
                using (var scope = ServiceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<ITimedReminderService>();
                    await service.TimedReminder();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        [Fact]
        public void TestCron()
        {
            var schedule = CrontabSchedule.Parse("50 15 * * *");
            var date = DateTime.Parse("2019-08-28 02:31:53.803");
            var nextTime = schedule.GetNextOccurrence(date);
            Output.WriteLine(nextTime.ToString(CultureInfo.InvariantCulture));
        }
    }
}