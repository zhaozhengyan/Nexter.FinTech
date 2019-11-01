using System;
using System.Linq;
using System.Threading.Tasks;
using FinTech.API.Wechat.Dto;
using FinTech.Domain;
using FinTech.Infrastructure;
using FinTech.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;

namespace FinTech.API.Wechat.Controllers
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
        public async Task<Result> GetAsync([FromQuery] TransactionQuery query)
        {
            var session = this.GetSession();
            var startDate = DateTime.Now.FirstDayOfThisMonth();
            var endDate = DateTime.Now.LastDayOfThisMonth();
            if (query.Date.HasValue)
            {
                startDate = query.Date.Value.FirstDayOfThisMonth();
                endDate = query.Date.Value.LastDayOfThisMonth();
            }
            var selectCategory = await Store.AsQueryable<Category>()
                .FirstOrDefaultAsync(e => e.Id == query.CategoryId);
            var queryable = from e in Store.AsQueryable<Transaction>()
                            join category in Store.AsQueryable<Category>() on e.CategoryId equals category.Id
                            join account in Store.AsQueryable<Account>() on e.AccountId equals account.Id
                            where e.MemberId == session.Id
                            && e.Date >= startDate && e.Date <= endDate
                            select new { e, category, account };
            if (selectCategory != null)
                queryable = queryable.Where(e => e.e.CategoryId == selectCategory.Id);
            var count = await queryable.LongCountAsync();
            var result = await queryable.Skip(query.Skip).Take(query.Take).ToListAsync();
            return Result.Complete(new
            {
                Count = count,
                DateMonth = result.FirstOrDefault()?.e.Date.ToString("yyyy-MM"),
                MonthTotalMoneys = new[]
                {
                   result.Sum(t => t.e.Spending),
                   result.Sum(t => t.e.Income)
                },
                @Lists = result.OrderByDescending(e => e.e.Date).ThenByDescending(e => e.e.CreatedAt).GroupBy(e => e.e.Date).Select(c => new
                {
                    Date = c.Key.ToString("yyyy-MM-dd"),
                    totalMoney = new[]
                         {
                        c.Where(e => e.e.Date == c.Key).Sum(t => t.e.Income),
                        c.Where(e => e.e.Date == c.Key).Sum(t => t.e.Spending),
                        0
                    },
                    @List = c.Where(e => e.e.Date == c.Key).Select(s => new
                    {
                        s.e.Id,
                        Date = s.e.Date.ToString("yyyy-MM-dd"),
                        Money = s.category.Type == CategoryType.Spending ? s.e.Spending : s.e.Income,
                        CategoryType = s.category.Type,
                        type = s.category.Type.GetDescription(),
                        categoryName = s.category.Name,
                        categoryIcon = s.category.Icon,
                        accountName = new[] { s.account.Name },
                        createTime = s.e.CreatedAt,
                        note = s.e.Memo
                    })
                })
            });
        }

        [HttpGet]
        [Route("Detail")]
        public async Task<Result> GetAsync([FromQuery]long id)
        {
            var session = this.GetSession();
            var queryable = from e in Store.AsQueryable<Transaction>()
                            join category in Store.AsQueryable<Category>() on e.CategoryId equals category.Id
                            join account in Store.AsQueryable<Account>() on e.AccountId equals account.Id
                            where e.MemberId == session.Id
                            where e.Id == id
                            select new { e, category, account };
            var s = await queryable.FirstOrDefaultAsync();
            return Result.Complete(new
            {
                s.e.Id,
                Date = s.e.Date.ToString("yyyy-MM-dd"),
                Money = s.category.Type == CategoryType.Spending ? s.e.Spending : s.e.Income,
                type = s.category.Type.GetDescription(),
                categoryName = s.category.Name,
                categoryIcon = s.category.Icon,
                accountName = new[] { s.account.Name },
                createTime = s.e.CreatedAt,
                note = s.e.Memo
            });
        }

        [HttpPost]
        public async Task<Result> PostAsync([FromBody] TransactionRequest request)
        {
            var session = this.GetSession();
            var category = await Store.AsQueryable<Category>()
                .FirstOrDefaultAsync(e => e.Id == request.CategoryId);
            var account = await Store.AsQueryable<Account>()
                .FirstOrDefaultAsync(e => e.Id == request.AccountId);
            decimal? income = null, spending = null;
            Rule.For(category).NotFound();
            Rule.For(account).NotFound();
            if (category.Type == CategoryType.Income)
                income = request.Money;
            else
                spending = request.Money;
            var trade = new Transaction(request.Memo,
                                        category.Id,
                                        request.AccountId,
                                        session.Id,
                                        spending,
                                        income,
                                        request.CreatedAt);
            await Store.AddAsync(trade);
            await Store.CommitAsync();
            return Result.Complete();
        }

        [HttpDelete]
        public async Task<Result> DeleteAsync([FromQuery]long id)
        {
            var session = this.GetSession();
            var queryable = from e in Store.AsQueryable<Transaction>()
                            where e.MemberId == session.Id
                            where e.Id == id
                            select e;
            var transaction = await queryable.FirstOrDefaultAsync();
            if (transaction == null) return Result.Complete();
            await Store.RemoveAsync(transaction);
            await Store.CommitAsync();
            return Result.Complete();
        }
    }
}
