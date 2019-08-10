using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinTech.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;

namespace Nexter.FinTech.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class MeController : ControllerBase
    {
        public MeController(IRepository store)
        {
            Store = store;
        }

        protected IRepository Store { get; }

        [HttpGet]
        public async Task<Result> GetAsync()
        {
            var session = this.GetSession();
            var queryable = from e in Store.AsQueryable<Member>()
                join account in Store.AsQueryable<Account>() on e.Id equals account.MemberId
                join transaction in Store.AsQueryable<Transaction>()
                    on e.Id equals transaction.MemberId into transactions
                from subTransaction in transactions.DefaultIfEmpty()
                select new { e, transactions };
            var result = await queryable.FirstOrDefaultAsync();

            return Result.Complete(new
            {
                totalIncome = result.transactions.Sum(e => e.Income),
                totalSpending = result.transactions.Sum(e => e.Spending),
                totalMoney = result.transactions.Sum(e => e.Income ?? 0 - e.Spending ?? 0),
                joinTime = result.e.CreatedAt,
                accountId = result.e.Id,
                accountName = result.e.NickName,
                openId = result.e.AccountCode,
                count = 100
            });
        }
    }
}
