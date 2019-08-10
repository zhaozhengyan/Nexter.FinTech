namespace FinTech.Domain
{

    public class Account
    {
        public long Id { get; set; } // Id (Primary key)
        public long MemberId { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 128)
        public AccountType? Type { get; set; } // Type
        public string Icon{ get; set; } 
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
