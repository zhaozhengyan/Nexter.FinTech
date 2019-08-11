using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinTech.Domain;
using FinTech.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nexter.Domain;

namespace Nexter.FinTech.Controllers
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
                accountName = result.e.NickName,
                openId = result.e.AccountCode,
                count = result.transactions.Count(),
                totalDays = day <= 0 ? 1 : day
            });
        }
        #endregion

        public class Auth
        {
            public string NickName { get; set; }
            public string Code { get; set; }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<Result> PostAsync([FromBody]Auth request)
        {
            StringBuilder url = new StringBuilder();
            url.Append($"https://api.weixin.qq.com/sns/jscode2session?appid={Appid}");
            url.Append($"&secret={Secret}");
            url.Append($"&js_code={request.Code}");
            url.Append("&grant_type=authorization_code");
            var res = await ExecuteAsync(url.ToString());
            if (res.HasValues)
            {
                var openId = res["openid"].ToString();
                var member = await Store.AsQueryable<Member>().FirstOrDefaultAsync(e => e.AccountCode == openId);
                if (member == null)
                {
                    member = new Member(100, request.NickName, openId);
                    await Store.AddAsync(member);
                    await Store.CommitAsync();
                }
                return Result.Complete(new { token = member.AccountCode });
            }
            return Result.Fail("注册帐户失败");
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
