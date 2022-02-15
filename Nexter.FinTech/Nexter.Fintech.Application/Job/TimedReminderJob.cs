using FinTech.ApplicationServices.Dto;
using FinTech.ApplicationServices.WeChat;
using FinTech.Domain;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DependencyInjection;
using Furion.Logging.Extensions;
using Microsoft.EntityFrameworkCore;
using Nexter.Fintech.Application.Wechat.Options;
using Nexter.Fintech.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nexter.Fintech.Application.Job
{
    public interface ITimedReminderJob
    {
        public Task Do();
    }
    public class TimedReminderJob : ITimedReminderJob, IScoped
    {
        private readonly IRepository<TimedReminder> _timedReminderRep;
        private readonly IRepository<Member> _memberRep;
        private IWechatApi _wechatApi;

        public TimedReminderJob(IRepository<TimedReminder> timedReminderRep, IRepository<Member> memberRep, IWechatApi wechatApi)
        {
            _timedReminderRep = timedReminderRep;
            _memberRep = memberRep;
            _wechatApi = wechatApi;
        }

        public async Task Do()
        {
            var opt = App.GetOptions<WechatOptions>();
            var now = DateTime.Now;
            var queryable = from e in _timedReminderRep.AsQueryable()
                    .Where(e => e.IsEnabled && e.LastReminderAt < now && e.FormId != null)
                            join member in _memberRep.AsQueryable() on e.MemberId equals member.Id
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
            var result = await _wechatApi.GetAccessToken(new GetAccessTokenRequest { Appid = opt.Appid, Secret = opt.Secret });
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
                        TemplateId = opt.TemplateId,
                        Page = opt.GoPage,
                        Data = new Data
                        {
                            Keyword1 = new Keyword { Value = opt.TemplateMsg },
                            Keyword2 = new Keyword { Value = $"{reminder.RegDays}天" },
                            Keyword3 = new Keyword { Value = lastReminderAt.ToString("yyyy-MM-dd") },
                        }
                    };
                    var messageRes = await _wechatApi.SendMessage(result.AccessToken, request);
                    if (messageRes.IsSuccess)
                        $"SendMessage[{reminder.openId}]:{messageRes.Msgid}".LogInformation<TimedReminderJob>();
                    else
                        $"SendMessage[{reminder.openId}]:{messageRes.Errcode}:{messageRes.Errmsg}".LogInformation<TimedReminderJob>();
                    reminder.e.SendSuccess();
                }
                await _timedReminderRep.SaveNowAsync();
            }
        }
    }
}
}
