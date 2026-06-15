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
    /// 物品管理
    /// </summary>
    [NonUnify]
    [Route("Item")]
    public class ItemAppService : IDynamicApiController
    {
        private readonly IRepository<Item> _itemRep;
        private readonly IRepository<ItemCategory> _categoryRep;

        public ItemAppService(IRepository<Item> itemRep, IRepository<ItemCategory> categoryRep)
        {
            _itemRep = itemRep;
            _categoryRep = categoryRep;
        }

        /// <summary>
        /// 获取物品分类列表
        /// </summary>
        [Route("Categories")]
        public async Task<Result> GetCategoriesAsync()
        {
            var session = App.HttpContext.Items["Session"] as Session;
            var categories = await _categoryRep.AsQueryable()
                .Where(e => e.MemberId == session.Id)
                .Select(e => new { e.Id, e.Name, e.Icon })
                .ToListAsync();

            return Result.Complete(categories);
        }

        /// <summary>
        /// 获取物品列表
        /// </summary>
        [Route("List")]
        public async Task<Result> GetListAsync([FromQuery] string sort, [FromQuery] long? categoryId)
        {
            var session = App.HttpContext.Items["Session"] as Session;

            var queryable = _itemRep.AsQueryable()
                .Where(e => e.MemberId == session.Id);

            if (categoryId.HasValue)
                queryable = queryable.Where(e => e.CategoryId == categoryId.Value);

            // 排序
            queryable = sort switch
            {
                "price" => queryable.OrderByDescending(e => e.Price),
                "name" => queryable.OrderBy(e => e.Name),
                "days" => queryable.OrderByDescending(e => e.PurchaseDate),
                _ => queryable.OrderByDescending(e => e.PurchaseDate)
            };

            var items = await queryable.ToListAsync();

            var result = items.Select(e => new
            {
                e.Id,
                e.Name,
                e.Price,
                e.AdditionalCost,
                e.PurchaseDate,
                e.RetireDate,
                e.WarrantyEndDate,
                e.Icon,
                e.PriceCalcMethod,
                e.Note,
                Days = (DateTime.Now - e.PurchaseDate).Days,
                DailyCost = (DateTime.Now - e.PurchaseDate).Days > 0
                    ? Math.Round(e.Price / (DateTime.Now - e.PurchaseDate).Days, 2)
                    : e.Price
            }).ToList();

            return Result.Complete(new
            {
                items = result,
                totalCount = result.Count,
                totalAsset = result.Sum(e => e.Price),
                avgDailyCost = result.Any() && result.Max(e => e.Days) > 0
                    ? Math.Round(result.Sum(e => e.Price) / result.Max(e => e.Days), 2)
                    : 0
            });
        }

        /// <summary>
        /// 获取物品详情
        /// </summary>
        [Route("Detail")]
        public async Task<Result> GetAsync([FromQuery] long id)
        {
            var session = App.HttpContext.Items["Session"] as Session;

            var item = await _itemRep.FirstOrDefaultAsync(e => e.Id == id && e.MemberId == session.Id);
            if (item == null) return Result.Fail("物品不存在");

            var days = (DateTime.Now - item.PurchaseDate).Days;

            return Result.Complete(new
            {
                item.Id,
                item.Name,
                item.Price,
                item.AdditionalCost,
                item.PurchaseDate,
                item.RetireDate,
                item.WarrantyEndDate,
                item.Icon,
                item.PriceCalcMethod,
                item.Note,
                Days = days,
                DailyCost = days > 0 ? Math.Round(item.Price / days, 2) : item.Price
            });
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        public async Task<Result> PostAsync([FromBody] ItemRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;

            var item = new Item(
                request.Name,
                request.Price,
                request.PurchaseDate,
                session.Id,
                request.CategoryId
            );

            item.AdditionalCost = request.AdditionalCost;
            item.RetireDate = request.RetireDate;
            item.WarrantyEndDate = request.WarrantyEndDate;
            item.Icon = request.Icon;
            item.PriceCalcMethod = request.PriceCalcMethod ?? "time";
            item.Note = request.Note;

            await _itemRep.InsertNowAsync(item);

            return Result.Complete(new { item.Id });
        }

        /// <summary>
        /// 更新物品
        /// </summary>
        public async Task<Result> PutAsync([FromBody] ItemRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;

            var item = await _itemRep.FirstOrDefaultAsync(e => e.Id == request.Id && e.MemberId == session.Id);
            if (item == null) return Result.Fail("物品不存在");

            item.Name = request.Name;
            item.Price = request.Price;
            item.AdditionalCost = request.AdditionalCost;
            item.PurchaseDate = request.PurchaseDate;
            item.RetireDate = request.RetireDate;
            item.WarrantyEndDate = request.WarrantyEndDate;
            item.CategoryId = request.CategoryId;
            item.Icon = request.Icon;
            item.PriceCalcMethod = request.PriceCalcMethod ?? "time";
            item.Note = request.Note;
            item.LastModifiedAt = DateTime.Now;

            await _itemRep.UpdateNowAsync(item);

            return Result.Complete();
        }

        /// <summary>
        /// 删除物品
        /// </summary>
        public async Task<Result> DeleteAsync([FromQuery] long id)
        {
            var session = App.HttpContext.Items["Session"] as Session;

            var item = await _itemRep.FirstOrDefaultAsync(e => e.Id == id && e.MemberId == session.Id);
            if (item == null) return Result.Complete();

            await _itemRep.DeleteNowAsync(item);

            return Result.Complete();
        }
    }

    public class ItemRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal AdditionalCost { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public long? CategoryId { get; set; }
        public string Icon { get; set; }
        public string PriceCalcMethod { get; set; }
        public string Note { get; set; }
    }
}
