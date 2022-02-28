using FinTech.ApplicationServices;
using FinTech.ApplicationServices.Dto;
using FinTech.ApplicationServices.WeChat;
using FinTech.Domain;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Fintech.Application.Wechat.Options;
using Nexter.Fintech.Core;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Nexter.Fintech.Application
{

    /// <summary>
    /// Me
    /// </summary>
    public class MeAppService : IDynamicApiController
    {
        private readonly IRepository<Member> _memberRep;
        private readonly IRepository<Account> _accountRep;
        private readonly IRepository<Transaction> _transcationRep;
        private readonly IRepository<TimedReminder> _timedReminderRep;
        private IWechatApi _wechatApi;

        public MeAppService(IRepository<Member> memberRep, IRepository<Account> accountRep, IRepository<Transaction> transcationRep, IRepository<TimedReminder> timedReminderRep, IWechatApi wechatApi)
        {
            _memberRep = memberRep;
            _accountRep = accountRep;
            _transcationRep = transcationRep;
            _timedReminderRep = timedReminderRep;
            _wechatApi = wechatApi;
        }

        /// <summary>
        /// ME
        /// </summary>
        /// <returns></returns>
        [NonUnify]
        public async Task<Result> GetAsync()
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var member = await _memberRep.FirstOrDefaultAsync(e => e.Id == session.Id);
            var transcations = await _transcationRep.Where(e => e.MemberId == member.Id).ToListAsync();
            var reminder = await _timedReminderRep.FirstOrDefaultAsync(e => e.MemberId == member.Id);
            var accounts = await _accountRep.AsQueryable().ToListAsync();
            var day = Math.Round(DateTime.Now.Subtract(member.CreatedAt).TotalDays, MidpointRounding.AwayFromZero);
            return Result.Complete(new
            {
                totalIncome = transcations.Sum(e => e.Income),
                totalSpending = transcations.Sum(e => e.Spending),
                totalMoney = transcations.Sum(e => e.Income ?? 0 - e.Spending ?? 0),
                joinTime = member.CreatedAt,
                accountId = member.Id,
                groupId = member.GroupId,
                accountName = member.NickName,
                openId = member.AccountCode,
                count = transcations.Count,
                totalDays = day <= 0 ? 1 : day,
                avatarUrl = member.Avatar,
                member.NickName,
                reminderTime = reminder?.GetNexterExecuteTime()?.ToString("t", CultureInfo.CurrentCulture)
            });
        }


        [NonUnify]
        [Route("TimedReminder")]
        public async Task<Result> TimedReminder([FromBody] TimedReminderRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var reminder = await _timedReminderRep.FirstOrDefaultAsync(e => e.MemberId == session.Id);
            if (reminder == null)
            {
                reminder = new TimedReminder(session.Id, request.Time);

                await _timedReminderRep.InsertAsync(reminder);
            }
            reminder.SetCron(request.Time);
            reminder.SetEnabled(request.IsEnabled);
            reminder.SetFormId(request.FormId);
            await _timedReminderRep.SaveNowAsync();
            return Result.Complete();
        }
        [NonUnify]
        [AllowAnonymous]
        public async Task<Result> PostAsync([FromBody] Auth request)
        {
            var openId = App.HttpContext.Request.Headers["Token"];
            var wechatOpt = App.GetOptions<WechatOptions>();
            if (string.IsNullOrWhiteSpace(openId))
            {
                var response = await _wechatApi.GetOpenId(new GetOpenIdRequest(wechatOpt.Appid, wechatOpt.Secret, request.Code));
                openId = response?.OpenId;
            }
            if (openId.NotAny())
                return Result.Fail("注册失败");
            var member = await _memberRep.AsQueryable().FirstOrDefaultAsync(e => e.AccountCode == openId);
            if (member == null)
            {
                member = new Member(request.NickName, openId, request.Avatar);
                await _memberRep.InsertAsync(member);
            }
            else
            {
                member.NickName = request.NickName;
                member.Avatar = request.Avatar;
            }
            var inviterGroupId = await InviterGroupId(request);
            if (inviterGroupId > 0)
                member.SetGroup(inviterGroupId);
            await _memberRep.SaveNowAsync();
            return Result.Complete(new { token = member.AccountCode });
        }


        private async Task<long> InviterGroupId(Auth request)
        {
            if (!string.IsNullOrWhiteSpace(request.InviterId))
            {
                var inviter = await _memberRep.AsQueryable()
                    .FirstOrDefaultAsync(e => e.AccountCode == request.InviterId);
                return inviter.GroupId;
            }
            return 0;
        }
    }
}