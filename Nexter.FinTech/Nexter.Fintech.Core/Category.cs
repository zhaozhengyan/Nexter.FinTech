
using Furion.DatabaseAccessor;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("Categories")]
    public class Category : IEntity
    {
        public Category() { }

        public Category(string name, string icon, CategoryType? type, long createMemberId)
        {
            Name = name;
            Icon = icon;
            Type = type;
            CreateMemberId = createMemberId;
        }

        public void SetName(string newName)
        {
            Name = newName;
        }

        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 32)
        public string Icon { get; set; } // Icon (length: 64)

        ///<summary>
        /// 1=Spending , 2= Income
        ///</summary>
        public CategoryType? Type { get; set; } // Type
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long? CreateMemberId { get; set; }
    }

    public enum CategoryType
    {
        [Description("支出")]
        Spending,
        [Description("收入")]
        Income,
        [Description("转账")]
        Transfer
    }
}

