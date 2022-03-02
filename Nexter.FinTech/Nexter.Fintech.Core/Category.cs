
using Furion.DatabaseAccessor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("Categories")]
    public class Category : BaseEntity,IEntityTypeBuilder<Category>
    {
        public Category() { }

        public Category(string name, string icon, CategoryType? type, long createMemberId)
        {
            Name = name;
            Icon = icon;
            Type = (int)type;
            CreateMemberId = createMemberId;
        }

        public void SetName(string newName)
        {
            Name = newName;
        }

        public string Name { get; set; } // Name (length: 32)
        public string Icon { get; set; } // Icon (length: 64)

        ///<summary>
        /// 1=Spending , 2= Income
        ///</summary>
        public int Type { get; set; } // Type
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long? CreateMemberId { get; set; }

        public void Configure(EntityTypeBuilder<Category> entityBuilder, DbContext dbContext, Type dbContextLocator)
        {
        }
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

