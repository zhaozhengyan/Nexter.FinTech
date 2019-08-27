using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FinTech.ApplicationServices.Dto;
using FinTech.Domain;
using FinTech.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nexter.Domain;
using Refit;

namespace FinTech.ApplicationServices
{
    public interface ITimedReminderService
    {
        Task TimedReminder();
    }
    public class TimedReminderService : BaseService, ITimedReminderService
    {
        public TimedReminderService(ILogger<TimedReminder> logger, IUnitOfWork unitOfWork, IRepository repository,
            IConfiguration configuration, IWeChatApi wechatApi)
            : base(logger, unitOfWork, repository)
        {
            WechatApi = wechatApi;
            Appid = configuration["Wechat:Appid"];
            Secret = configuration["Wechat:Secret"];
            TemplateId = configuration["Wechat:MsgTemplateId"];
            Page = configuration["Wechat:GoPage"];
            TemplateMsg = configuration["Wechat:MsgContent"];

        }
        protected string Appid { get; }
        protected string Page { get; }
        protected string Secret { get; }
        protected string TemplateId { get; }
        protected string TemplateMsg { get; }
        protected IWeChatApi WechatApi { get; }


        public async Task TimedReminder()
        {
            var now = DateTime.Now;
            var queryable = from e in Repository.AsQueryable<TimedReminder>()
                    .Where(e => e.IsEnabled && e.LastReminderAt < now.Date && e.FormId.IsAny())
                            join member in Repository.AsQueryable<Member>() on e.MemberId equals member.Id
                            select new
                            {
                                e,
                                openId = member.AccountCode,
                                RegDays = member.GetRegDays(),
                                TimeCron = e.GetNexterExecuteTime(),
                                e.FormId,
                                e.LastReminderAt
                            };
            var reminders = await queryable.ToListAsync();
            if (reminders.NotAny()) return;
            var result = await WechatApi.GetAccessToken(new GetAccessTokenRequest { Appid = Appid, Secret = Secret });
            if (result?.ExpiresIn > 0)
            {
                foreach (var reminder in reminders)
                {
                    if (now <= reminder.TimeCron) continue;
                    var lastReminderAt = reminder.LastReminderAt ?? now;
                    var request = new SendMessageRequest
                    {
                        ToUser = reminder.openId,
                        FormId = reminder.FormId,
                        TemplateId = TemplateId,
                        Page = Page,
                        Data = new Data
                        {
                            Keyword1 = new Keyword { Value = TemplateMsg },
                            Keyword2 = new Keyword { Value = $"{reminder.RegDays}天" },
                            Keyword3 = new Keyword { Value = lastReminderAt.ToString("yyyy-MM-dd") },
                        }
                    };
                    var messageRes = await WechatApi.SendMessage(result.AccessToken, request);
                    if (messageRes.IsSuccess)
                        Logger.LogInformation($"SendMessage[{reminder.openId}]:{messageRes.Msgid}");
                    else
                        Logger.LogError($"SendMessage[{reminder.openId}]:{messageRes.Errcode}:{messageRes.Errmsg}");
                    reminder.e.ClearFormId();
                }
                await Repository.CommitAsync();
            }
        }
    }
}
