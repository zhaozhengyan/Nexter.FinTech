using Furion.DatabaseAccessor;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("Items")]
    public class Item : BaseEntity
    {
        public Item() { }

        public Item(string name, decimal price, DateTime purchaseDate, long memberId, long? categoryId)
        {
            Name = name;
            Price = price;
            PurchaseDate = purchaseDate;
            MemberId = memberId;
            CategoryId = categoryId;
            CreatedAt = DateTime.Now;
            LastModifiedAt = DateTime.Now;
        }

        /// <summary>
        /// 物品名称
        /// </summary>
        [MaxLength(64)]
        public string Name { get; set; }

        /// <summary>
        /// 购买价格
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// 附加费用
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalCost { get; set; }

        /// <summary>
        /// 购买日期
        /// </summary>
        public DateTime PurchaseDate { get; set; }

        /// <summary>
        /// 退役日期（可选）
        /// </summary>
        public DateTime? RetireDate { get; set; }

        /// <summary>
        /// 过保日期（可选）
        /// </summary>
        public DateTime? WarrantyEndDate { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long MemberId { get; set; }

        /// <summary>
        /// 分类ID
        /// </summary>
        public long? CategoryId { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [MaxLength(32)]
        public string Icon { get; set; }

        /// <summary>
        /// 计算均价方式: time=按时间, usage=按使用次数
        /// </summary>
        [MaxLength(16)]
        public string PriceCalcMethod { get; set; } = "time";

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(256)]
        public string Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
