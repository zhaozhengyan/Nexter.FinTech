using Furion.DatabaseAccessor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("ItemCategories")]
    public class ItemCategory : BaseEntity
    {
        public ItemCategory() { }

        public ItemCategory(string name, string icon, long memberId)
        {
            Name = name;
            Icon = icon;
            MemberId = memberId;
        }

        /// <summary>
        /// 分类名称
        /// </summary>
        [MaxLength(32)]
        public string Name { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [MaxLength(32)]
        public string Icon { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long? MemberId { get; set; }
    }
}
