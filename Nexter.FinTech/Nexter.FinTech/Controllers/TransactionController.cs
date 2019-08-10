using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinTech.Domain;
using FinTech.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;

namespace Nexter.FinTech.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        public TransactionController(IRepository store)
        {
            Store = store;
        }

        protected IRepository Store { get; }

        [HttpGet]
        public async Task<Result> GetAsync(int skip = 0, int take = 10)
        {
            var session = this.GetSession();
            var queryable = from e in Store.AsQueryable<Transaction>()
                            join category in Store.AsQueryable<Category>() on e.CategoryId equals category.Id
                            join account in Store.AsQueryable<Account>() on e.AccountId equals account.Id
                            //where e.MemberId == session.Id
                            select new { e, category, account };
            var result = await queryable.Skip(skip).Take(take).ToListAsync();
            return Result.Complete(new
            {
                DateMonth = result.FirstOrDefault()?.e.Date.ToString("yyyy-MM"),
                MonthTotalMoneys = new[]
                {
                   result.Sum(t => t.e.Income),
                   result.Sum(t => t.e.Spending),
                },
                @Lists = result.GroupBy(e => e.e.Date).Select(c => new
                {
                    DateMonth = result.FirstOrDefault()?.e.Date.ToString("yyyy-MM-dd"),
                    totalMoney = new[]
                     {
                        c.Where(e => e.e.Date == c.Key).Sum(t => t.e.Income),
                        c.Where(e => e.e.Date == c.Key).Sum(t => t.e.Spending),
                        0
                    },
                    @List = c.Where(e => e.e.Date == c.Key).Select(s => new
                    {
                        s.e.Id,
                        s.e.Date,
                        s.e.Income,
                        s.e.Spending,
                        type = s.category.Type.GetDescription(),
                        categoryName = s.category.Name,
                        categoryIcon = s.category.Icon,
                        accountName = new string[] { s.account.Name },
                        createTime = s.e.CreatedAt,
                        note = s.e.Memo
                    })
                })
            });
        }
    }
}
