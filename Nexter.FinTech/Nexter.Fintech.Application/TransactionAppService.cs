using FinTech.Domain;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Fintech.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Nexter.Fintech.Application
{
    /// <summary>
    /// Transcation
    /// </summary>
    public class TransactionAppService : IDynamicApiController
    {
        private readonly IRepository<Category> _categoryRep;
        private readonly IRepository<Account> _accountRep;
        private readonly IRepository<Transaction> _transcationRep;

        public TransactionAppService(IRepository<Category> categoryRep, IRepository<Account> accountRep, IRepository<Transaction> transcationRep)
        {
            _categoryRep = categoryRep;
            _accountRep = accountRep;
            _transcationRep = transcationRep;
        }

        public async Task<Result> GetAsync([FromQuery] TransactionQuery query)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var startDate = DateTime.Now.FirstDayOfThisMonth();
            var endDate = DateTime.Now.LastDayOfThisMonth();
            if (query.Date.HasValue)
            {
                startDate = query.Date.Value.FirstDayOfThisMonth();
                endDate = query.Date.Value.LastDayOfThisMonth();
            }
            var selectCategory = await _categoryRep.FirstOrDefaultAsync(e => e.Id == query.CategoryId);
            var queryable = from e in _transcationRep.AsQueryable()
                            join category in _categoryRep.AsQueryable() on e.CategoryId equals category.Id
                            join account in _accountRep.AsQueryable() on e.AccountId equals account.Id
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

        [Route("Detail")]
        public async Task<Result> GetAsync([FromQuery] long id)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var queryable = from e in _transcationRep.AsQueryable()
                            join category in _categoryRep.AsQueryable() on e.CategoryId equals category.Id
                            join account in _accountRep.AsQueryable() on e.AccountId equals account.Id
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

        public async Task<Result> PostAsync([FromBody] TransactionRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var category = await _categoryRep.AsQueryable()
                .FirstOrDefaultAsync(e => e.Id == request.CategoryId);
            var account = await _accountRep.AsQueryable()
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
            await _transcationRep.InsertNowAsync(trade);
            return Result.Complete();
        }

        public async Task<Result> DeleteAsync([FromQuery] long id)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var queryable = from e in _transcationRep.AsQueryable()
                            where e.MemberId == session.Id
                            where e.Id == id
                            select e;
            var transaction = await queryable.FirstOrDefaultAsync();
            if (transaction == null) return Result.Complete();
            await _transcationRep.DeleteNowAsync(transaction);
            return Result.Complete();
        }
    }
}