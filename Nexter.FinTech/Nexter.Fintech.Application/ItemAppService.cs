using FinTech.Domain;
using Furion;
using Furion.DatabaseAccessor;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nexter.Fintech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        public async Task<Result> GetListAsync([FromQuery] string sort, [FromQuery] long? categoryId, [FromQuery] string status, [FromQuery] string name)
        {
            var session = App.HttpContext.Items["Session"] as Session;

            var queryable = _itemRep.AsQueryable()
                .Where(e => e.MemberId == session.Id);

            if (categoryId.HasValue)
                queryable = queryable.Where(e => e.CategoryId == categoryId.Value);

            // 名称模糊搜索
            if (!string.IsNullOrEmpty(name))
                queryable = queryable.Where(e => e.Name.Contains(name));

            // 状态筛选
            var now = DateTime.Now;
            if (status == "active")
                queryable = queryable.Where(e => !e.RetireDate.HasValue || e.RetireDate > now);
            else if (status == "retired")
                queryable = queryable.Where(e => e.RetireDate.HasValue && e.RetireDate <= now);

            // 排序
            queryable = sort switch
            {
                "price" => queryable.OrderByDescending(e => e.Price),
                "priceAsc" => queryable.OrderBy(e => e.Price),
                "name" => queryable.OrderBy(e => e.Name),
                "days" => queryable.OrderByDescending(e => e.PurchaseDate),
                "daysAsc" => queryable.OrderBy(e => e.PurchaseDate),
                _ => queryable
                    .OrderByDescending(e => e.SortOrder > 0 ? 1 : 0)
                    .ThenBy(e => e.SortOrder == 0 ? int.MaxValue : e.SortOrder)
                    .ThenByDescending(e => e.PurchaseDate)
            };

            var items = await queryable.ToListAsync();
            var now2 = DateTime.Now;

            var result = items.Select(e =>
            {
                var days = (now2 - e.PurchaseDate).Days;
                var additionalItems = ParseAdditionalItems(e.AdditionalItemsJson);
                var effectiveCost = CalcEffectiveCost(e.Price, additionalItems);
                return new
                {
                    e.Id,
                    e.Name,
                    e.Price,
                    e.AdditionalCost,
                    AdditionalItems = additionalItems,
                    e.PurchaseDate,
                    e.RetireDate,
                    e.WarrantyEndDate,
                    e.Icon,
                    e.PriceCalcMethod,
                    e.Note,
                    Days = days,
                    DailyCost = days > 0 ? Math.Round(effectiveCost / days, 2) : effectiveCost,
                    EffectiveCost = effectiveCost,
                    IsRetired = e.RetireDate.HasValue && e.RetireDate.Value <= now2
                };
            }).ToList();

            // totalAsset 仅统计现役物品
            var activeItems = result.Where(e => !e.IsRetired).ToList();
            return Result.Complete(new
            {
                items = result,
                totalCount = result.Count,
                totalAsset = activeItems.Sum(e => e.EffectiveCost),
                avgDailyCost = activeItems.Any() && activeItems.Max(e => e.Days) > 0
                    ? Math.Round(activeItems.Sum(e => e.EffectiveCost) / activeItems.Max(e => e.Days), 2)
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
            var additionalItems = ParseAdditionalItems(item.AdditionalItemsJson);
            var effectiveCost = CalcEffectiveCost(item.Price, additionalItems);

            return Result.Complete(new
            {
                item.Id,
                item.Name,
                item.Price,
                item.AdditionalCost,
                AdditionalItems = additionalItems,
                item.PurchaseDate,
                item.RetireDate,
                item.WarrantyEndDate,
                item.Icon,
                item.PriceCalcMethod,
                item.Note,
                Days = days,
                DailyCost = days > 0 ? Math.Round(effectiveCost / days, 2) : effectiveCost,
                EffectiveCost = effectiveCost,
                IsRetired = item.RetireDate.HasValue && item.RetireDate.Value <= DateTime.Now
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
            item.AdditionalItemsJson = request.AdditionalItemsJson;
            item.RetireDate = request.RetireDate;
            item.WarrantyEndDate = request.WarrantyEndDate;
            item.Icon = request.Icon;
            item.PriceCalcMethod = request.PriceCalcMethod ?? "time";
            item.Note = request.Note;

            // Auto-assign SortOrder (max + 1)
            var maxSort = await _itemRep.AsQueryable()
                .Where(e => e.MemberId == session.Id)
                .MaxAsync(e => (int?)e.SortOrder) ?? 0;
            item.SortOrder = maxSort + 1;

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
            item.AdditionalItemsJson = request.AdditionalItemsJson;
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

        /// <summary>
        /// 批量更新自定义排序
        /// </summary>
        [HttpPut]
        [Route("Reorder")]
        public async Task<Result> ReorderAsync([FromBody] ReorderRequest request)
        {
            var session = App.HttpContext.Items["Session"] as Session;

            if (request.Ids == null || request.Ids.Count == 0)
                return Result.Fail("排序列表不能为空");

            var items = await _itemRep.AsQueryable()
                .Where(e => e.MemberId == session.Id)
                .ToListAsync();

            for (int i = 0; i < request.Ids.Count; i++)
            {
                var item = items.FirstOrDefault(e => e.Id == request.Ids[i]);
                if (item != null)
                {
                    item.SortOrder = i + 1;
                    await _itemRep.UpdateNowAsync(item);
                }
            }

            return Result.Complete();
        }

        /// <summary>
        /// 解析附加项 JSON
        /// </summary>
        private static List<AdditionalItemDto> ParseAdditionalItems(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<AdditionalItemDto>();
            try
            {
                return JsonSerializer.Deserialize<List<AdditionalItemDto>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new List<AdditionalItemDto>();
            }
            catch { return new List<AdditionalItemDto>(); }
        }

        /// <summary>
        /// 计算实际花费 = 价格 + 支出附加项 - 收入附加项
        /// </summary>
        private static decimal CalcEffectiveCost(decimal price, List<AdditionalItemDto> items)
        {
            if (items == null || items.Count == 0) return price;
            var income = items.Where(i => i.Type == "income").Sum(i => i.Amount);
            var expense = items.Where(i => i.Type == "expense").Sum(i => i.Amount);
            return price + expense - income;
        }
    }

    public class ItemRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal AdditionalCost { get; set; }
        public string AdditionalItemsJson { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? RetireDate { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public long? CategoryId { get; set; }
        public string Icon { get; set; }
        public string PriceCalcMethod { get; set; }
        public string Note { get; set; }
    }

    public class AdditionalItemDto
    {
        public string Name { get; set; }
        public string Type { get; set; } // "income" or "expense"
        public decimal Amount { get; set; }
        public string Date { get; set; }
    }

    public class ReorderRequest
    {
        public List<long> Ids { get; set; }
    }
}
