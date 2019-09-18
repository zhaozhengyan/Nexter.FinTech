using FinTech.API.Wechat.Dto;
using FinTech.ApplicationServices;
using FinTech.ApplicationServices.Dto;
using FinTech.Domain;
using FinTech.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nexter.Domain;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace FinTech.API.Wechat.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        public MeController(IRepository store, IConfiguration configuration, IWeChatApi wechatApi)
        {
            Store = store;
            WechatApi = wechatApi;
            AuthUrl = configuration["Wechat:AuthUrl"];
            Appid = configuration["Wechat:Appid"];
            Secret = configuration["Wechat:Secret"];
        }
        protected IRepository Store { get; }
        protected string AuthUrl { get; }
        protected string Appid { get; }
        protected string Secret { get; }
        protected IWeChatApi WechatApi { get; }
        #region UserInfo
        [HttpGet]
        public async Task<Result> GetAsync()
        {
            var session = this.GetSession();
            var queryable = from e in Store.AsQueryable<Member>()
                            join transaction in Store.AsQueryable<Transaction>()
                                on e.Id equals transaction.MemberId into transactions
                            from subTransaction in transactions.DefaultIfEmpty()
                            join r in Store.AsQueryable<TimedReminder>() on e.Id equals r.MemberId into temp
                            from reminder in temp.DefaultIfEmpty()
                            where e.Id == session.Id
                            select new { e, reminder, transactions };
            var result = await queryable.FirstOrDefaultAsync();
            var day = Math.Round(DateTime.Now.Subtract(result.e.CreatedAt).TotalDays, MidpointRounding.AwayFromZero);
            return Result.Complete(new
            {
                totalIncome = result.transactions.Sum(e => e.Income),
                totalSpending = result.transactions.Sum(e => e.Spending),
                totalMoney = result.transactions.Sum(e => e.Income ?? 0 - e.Spending ?? 0),
                joinTime = result.e.CreatedAt,
                accountId = result.e.Id,
                groupId = result.e.GroupId,
                accountName = result.e.NickName,
                openId = result.e.AccountCode,
                count = result.transactions.Count(),
                totalDays = day <= 0 ? 1 : day,
                reminderTime = result.reminder?.GetNexterExecuteTime()?.ToString("t", CultureInfo.CurrentCulture)
            });
        }
        #endregion

        [HttpPost]
        [Route("TimedReminder")]
        public async Task<Result> TimedReminder([FromBody] TimedReminderRequest request)
        {
            var session = this.GetSession();
            var reminder = await Store.AsQueryable<TimedReminder>().FirstOrDefaultAsync(e => e.MemberId == session.Id);
            if (reminder == null)
            {
                reminder = new TimedReminder(session.Id, request.Time);
                await Store.AddAsync(reminder);
            }
            reminder.SetCron(request.Time);
            reminder.SetEnabled(request.IsEnabled);
            reminder.SetFormId(request.FormId);
            await Store.CommitAsync();
            return Result.Complete();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<Result> PostAsync([FromBody]Auth request)
        {
            var openId = Request.Headers["Token"];
            if (string.IsNullOrWhiteSpace(openId))
            {
                var response = await WechatApi.GetOpenId(new GetOpenIdRequest(Appid, Secret, request.Code));
                openId = response?.OpenId;
            }
            if (openId.NotAny())
                return Result.Fail("注册失败");
            var member = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.AccountCode == openId);
            if (member == null)
            {
                member = new Member(request.NickName, openId, request.Avatar);
                await Store.AddAsync(member);
            }
            else
            {
                member.NickName = request.NickName;
                member.Avatar = request.Avatar;
            }
            var inviterGroupId = await InviterGroupId(request);
            if (inviterGroupId > 0)
                member.SetGroup(inviterGroupId);
            await Store.CommitAsync();
            return Result.Complete(new { token = member.AccountCode });
        }


        private async Task<long> InviterGroupId(Auth request)
        {
            if (!string.IsNullOrWhiteSpace(request.InviterId))
            {
                var inviter = await Store.AsQueryable<Member>()
                    .FirstOrDefaultAsync(e => e.AccountCode == request.InviterId);
                return inviter.GroupId;
            }
            return 0;
        }

    }
}
