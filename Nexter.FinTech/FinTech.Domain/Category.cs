
using System.ComponentModel;

namespace FinTech.Domain
{

    public class Category
    {
        public long Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 32)
        public string Icon { get; set; } // Icon (length: 64)

        ///<summary>
        /// 1=Spending , 2= Income
        ///</summary>
        public CategoryType? Type { get; set; } // Type
    }

    public enum CategoryType
    {
        [Description("支出")]
        Spending,
        [Description("收入")]
        Income 
    }
}

