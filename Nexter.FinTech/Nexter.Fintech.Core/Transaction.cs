using Furion.DatabaseAccessor;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTech.Domain
{
    [Table("Transactions")]
    public class Transaction : BaseEntity
    {
        public Transaction() { }

        /// <summary>
        /// 交易对象
        /// </summary>
        /// <param name="memo">备注</param>
        /// <param name="categoryId"></param>
        /// <param name="accountId">支付宝帐户ID</param>s
        /// <param name="memberId">会员ID</param>
        /// <param name="spending">花销</param>
        /// <param name="income">收入</param>
        /// <param name="createdAt"></param>
        public Transaction(string memo, long categoryId, long accountId, long memberId, decimal? spending, decimal? income, DateTime createdAt)
        {
            Memo = memo;
            AccountId = accountId;
            MemberId = memberId;
            BookId = 0;
            CategoryId = categoryId;
            Spending = spending ?? 0;
            Income = income ?? 0;
            Date = createdAt.Date;
            CreatedAt = DateTime.Now;
            LastModifiedAt = DateTime.Now;
        }

        public string Memo { get; set; } // Memo (length: 128)
        public long AccountId { get; set; } // AccountId
        public long MemberId { get; set; } // MemberId
        public long CategoryId { get; set; } // CategoryId

        ///<summary>
        /// 账本ID
        ///</summary>
        public long BookId { get; set; } // BookId

        ///<summary>
        /// 花销
        ///</summary>
        public decimal? Spending { get; set; } // Spending

        ///<summary>
        /// 收入
        ///</summary>
        public decimal? Income { get; set; } // Income

        ///<summary>
        /// 日期
        ///</summary>
        public DateTime Date { get; set; } // Date
        public DateTime CreatedAt { get; set; } // CreatedAt
        public DateTime LastModifiedAt { get; set; } // LastModifiedAt
    }

}
