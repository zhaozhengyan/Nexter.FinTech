using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinTech.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Nexter.Domain;

namespace Nexter.FinTech.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public CategoryController(IRepository store)
        {
            Store = store;
        }

        protected IRepository Store { get; }

        [HttpGet]
        public async Task<Result> GetAsync()
        {
            var result = await Store.AsQueryable<Category>().ToListAsync();
            return Result.Complete(new
            {
                categorys = new object[]
                {
                    new {
                        tallyType = "支出",
                        category = result.Where(e => e.Type == CategoryType.Spending).ToList()
                    },
                    new {
                        tallyType = "收入",
                        category = result.Where(e => e.Type == CategoryType.Income).ToList()
                    }
                }

            });
        }
    }
}
