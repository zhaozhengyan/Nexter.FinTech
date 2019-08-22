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
            var session = this.GetSession();
            var ids = new[] { 0, session.Id };
            var result = await Store.AsQueryable<Category>().Where(c => ids.Contains(c.CreateMemberId.Value)).ToListAsync();
            return Result.Complete(new
            {
                categorys = new object[]
                {
                    new {
                        tallyType = "支出",
                        category = result.Where(e => e.Type == CategoryType.Spending).Select(e=>new
                        {
                            categoryId=e.Id,
                            categoryName=e.Name,
                            categoryIcon=e.Icon,
                            e.Type,
                            IsAdmin=e.CreateMemberId==session.Id
                        })
                        .ToList()
                    },
                    new {
                        tallyType = "收入",
                        category = result.Where(e => e.Type == CategoryType.Income).Select(e=>new
                        {
                            categoryId=e.Id,
                            categoryName=e.Name,
                            categoryIcon=e.Icon,
                            e.Type,
                            IsAdmin=e.CreateMemberId==session.Id
                        })
                        .ToList()
                    }
                }

            });
        }

        [HttpPost]
        public async Task<Result> PostAsync([FromBody]CategoryRequest request)
        {
            var session = this.GetSession();
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Complete();
            var category = new Category(request.Name, request.Icon, request.Type, session.Id);
            await Store.AddAsync(category);
            await Store.CommitAsync();
            return Result.Complete();
        }


        [HttpPut]
        public async Task<Result> PutAsync([FromBody]CategoryRequest request)
        {
            var session = this.GetSession();
            var cat = await Store.AsQueryable<Category>()
                .FirstOrDefaultAsync(e => e.CreateMemberId == session.Id && e.Id == request.Id);
            if (cat == null)
                return Result.Complete("Not Found");
            cat.SetName(request.Name);
            await Store.CommitAsync();
            return Result.Complete();
        }

        [HttpDelete]
        public async Task<Result> DeleteAsync([FromBody] CategoryRequest request)
        {
            var session = this.GetSession();
            var cat = await Store.AsQueryable<Category>()
                .FirstOrDefaultAsync(e => e.CreateMemberId == session.Id && e.Id == request.Id);
            if (cat == null)
                return Result.Complete("Not Found");
            await Store.RemoveAsync(cat);
            await Store.CommitAsync();
            return Result.Complete("删除成功");
        }


    }
}
