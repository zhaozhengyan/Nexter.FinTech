using FinTech.Domain;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Fintech.Core;
using System.Linq;
using System.Threading.Tasks;

namespace Nexter.Fintech.Application
{
    /// <summary>
    /// Account
    /// </summary>
    [NonUnify]
    public class AccountAppService : IDynamicApiController
    {
        private readonly IRepository<Account> _accountRep;
        private readonly IRepository<Transaction> _transcationRep;
        public AccountAppService(IRepository<Account> accountRep, IRepository<Transaction> transcationRep)
        {
            _accountRep = accountRep;
            _transcationRep = transcationRep;
        }

        public async Task<Result> GetAsync()
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var transcations = await _transcationRep.Where(e => e.MemberId == session.Id).ToListAsync();
            var accounts = await _accountRep.AsQueryable().ToListAsync();
            return Result.Complete(new
            {
                totalIncome = transcations.Sum(e => e.Income),
                totalSpending = transcations.Sum(e => e.Spending),
                totalMoney = transcations.Sum(e => e.Income ?? 0 - e.Spending ?? 0),
                account = accounts.Select(e => new
                {
                    accountId = e.Id,
                    accountName = e.Name,
                    iconPath = e.Icon,
                    money = transcations.Where(r => r.AccountId == e.Id).Sum(t => t.Income - t.Spending)
                })
            });
        }
    }
}