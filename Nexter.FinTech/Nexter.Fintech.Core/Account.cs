using Furion.DatabaseAccessor;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("Accounts")]
    public class Account : BaseEntity
    {
        public long MemberId { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 128)
        public string Type { get; set; } // Type
        public string Icon { get; set; }
    }

    public enum AccountType
    {
        Cash,
        Alipay,
        Wechat,
        Bank,
        Other
    }

}
