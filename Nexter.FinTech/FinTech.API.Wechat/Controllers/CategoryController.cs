using System.Linq;
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
                        category = result.Where(e => e.Type == CategoryType.Spending)
                        .Select(e=>new
                        {
                            categoryId=e.Id,
                            categoryName=e.Name,
                            categoryIcon=e.Icon
                        })
                        .ToList()
                    },
                    new {
                        tallyType = "收入",
                        category = result.Where(e => e.Type == CategoryType.Income).Select(e=>new
                        {
                            categoryId=e.Id,
                            categoryName=e.Name,
                            categoryIcon=e.Icon
                        })
                        .ToList()
                    }
                }

            });
        }
    }
}
