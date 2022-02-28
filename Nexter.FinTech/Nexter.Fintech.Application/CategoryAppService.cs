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
    /// Category
    /// </summary>
    [NonUnify]
    public class CategoryAppService : IDynamicApiController
    {
        private readonly IRepository<Transaction> _transcationRep;
        private readonly IRepository<Category> _categoryRep;

        public CategoryAppService(IRepository<Transaction> transcationRep, IRepository<Category> categoryRep)
        {
            _transcationRep = transcationRep;
            _categoryRep = categoryRep;
        }

        public async Task<Result> GetAsync()
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var ids = new[] { 0, session.Id };
            var result = await _categoryRep.AsQueryable().Where(c => ids.Contains(c.CreateMemberId.Value)).ToListAsync();
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

        public async Task<Result> PostAsync([FromBody] CategoryRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Complete();
            var category = new Category(request.Name, request.Icon, request.Type, session.Id);
            await _categoryRep.InsertNowAsync(category);
            return Result.Complete();
        }


        public async Task<Result> PutAsync([FromBody] CategoryRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var cat = await _categoryRep.AsQueryable()
                .FirstOrDefaultAsync(e => e.CreateMemberId == session.Id && e.Id == request.Id);
            if (cat == null)
                return Result.Complete("Not Found");
            cat.SetName(request.Name);
            await _categoryRep.SaveNowAsync();
            return Result.Complete();
        }

        public async Task<Result> DeleteAsync([FromBody] CategoryRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var cat = await _categoryRep.AsQueryable()
                .FirstOrDefaultAsync(e => e.CreateMemberId == session.Id && e.Id == request.Id);
            if (cat == null)
                return Result.Complete("Not Found");
            var hasTransaction = await _transcationRep.AsQueryable().AnyAsync(e => e.CategoryId == request.Id);
            if (hasTransaction)
                return Result.Fail(nameof(BusinessViolationStatusCodes.RuleViolated), "该分类下有记账，不能删除哦");
            await _categoryRep.DeleteNowAsync(cat);
            return Result.Complete("删除成功");
        }


    }
}