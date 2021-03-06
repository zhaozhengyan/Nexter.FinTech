﻿using System.Linq;
using System.Threading.Tasks;
using FinTech.API.Wechat.Dto;
using FinTech.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Domain;

namespace FinTech.API.Wechat.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(IRepository store)
        {
            Store = store;
        }

        protected IRepository Store { get; }

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
            var accounts = await Store.AsQueryable<Account>().ToListAsync();
            return Result.Complete(new
            {
                totalIncome = result.transactions.Sum(e => e.Income),
                totalSpending = result.transactions.Sum(e => e.Spending),
                totalMoney = result.transactions.Sum(e => e.Income ?? 0 - e.Spending ?? 0),
                account = accounts.Select(e => new
                {
                    accountId = e.Id,
                    accountName = e.Name,
                    iconPath = e.Icon,
                    money = result.transactions.Where(r => r.AccountId == e.Id).Sum(t => t.Income - t.Spending)
                })
            });
        }
    }
}
