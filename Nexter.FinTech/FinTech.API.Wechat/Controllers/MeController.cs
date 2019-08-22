using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinTech.API.Wechat.Dto;
using FinTech.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nexter.Domain;

namespace FinTech.API.Wechat.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        public MeController(IRepository store, IConfiguration configuration)
        {
            Store = store;
            Http = new HttpClient();
            AuthUrl = configuration["Wechat:AuthUrl"];
            Appid = configuration["Wechat:Appid"];
            Secret = configuration["Wechat:Secret"];
        }
        protected HttpClient Http { get; }
        protected IRepository Store { get; }
        protected string AuthUrl { get; }
        protected string Appid { get; }
        protected string Secret { get; }

        #region UserInfo
        [HttpGet]
        public async Task<Result> GetAsync()
        {
            var session = this.GetSession();
            var queryable = from e in Store.AsQueryable<Member>()
                            join transaction in Store.AsQueryable<Transaction>()
                                on e.Id equals transaction.MemberId into transactions
                            from subTransaction in transactions.DefaultIfEmpty()
                            where e.Id == session.Id
                            select new { e, transactions };
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
                totalDays = day <= 0 ? 1 : day
            });
        }
        #endregion

        [HttpPost]
        [AllowAnonymous]
        public async Task<Result> PostAsync([FromBody]Auth request)
        {
            var openId = Request.Headers["Token"];
            Member member;
            if (string.IsNullOrWhiteSpace(openId))
            {
                openId = await GetToken(request.Code);
                member = new Member(request.NickName, openId, request.Avatar);
                await Store.AddAsync(member);
            }
            else
            {
                member = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.AccountCode == openId);
                member.NickName = request.NickName;
                member.Avatar = request.Avatar;
            }
            var inviterGroupId = await InviterGroupId(request);
            if (inviterGroupId > 0)
                member.SetGroup(inviterGroupId);
            await Store.CommitAsync();
            return Result.Complete(new { token = member.AccountCode });
        }

        private async Task<string> GetToken(string code)
        {
            StringBuilder url = new StringBuilder();
            url.Append($"{AuthUrl}");
            url.Append($"?appid={Appid}");
            url.Append($"&secret={Secret}");
            url.Append($"&js_code={code}");
            url.Append("&grant_type=authorization_code");
            var res = await ExecuteAsync(url.ToString());
            if (res.HasValues)
            {
                return res["openid"].ToString();
            }
            return string.Empty;
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


        protected async Task<JObject> ExecuteAsync(string url)
        {
            var res = await Http.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();
            var responseJObject = JsonConvert.DeserializeObject<JObject>(content);
            return responseJObject;
        }
    }
}
