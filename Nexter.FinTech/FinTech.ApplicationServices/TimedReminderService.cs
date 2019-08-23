using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FinTech.Domain;
using FinTech.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nexter.Domain;

namespace FinTech.ApplicationServices
{
    public interface ITimedReminderService
    {
        Task TimedReminder(string baseUrl, string appid, string secret);
    }
    public class TimedReminderService : BaseService, ITimedReminderService
    {
        public TimedReminderService(ILogger<TimedReminder> logger, IUnitOfWork unitOfWork, IRepository repository)
            : base(logger, unitOfWork, repository)
        {
        }


        public async Task TimedReminder(string baseUrl, string appid, string secret)
        {
            var now = DateTime.Now;
            var reminders = await Repository.AsQueryable<TimedReminder>().Where(e => e.IsEnabled && e.LastReminderAt < now.Date).ToListAsync();
            if (reminders.NotAny()) return;
            var url = $"{baseUrl}?grant_type=client_credential&appid={appid}&secret={secret}";
            var result = await ExecuteAsync(url);
            if (result.HasValues)
            {
                var token = result["access_token"].ToString();
            }
        }

        protected async Task<JObject> ExecuteAsync(string url)
        {
            var http = new HttpClient();
            var res = await http.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();
            var responseJObject = JsonConvert.DeserializeObject<JObject>(content);
            return responseJObject;
        }
    }
}
