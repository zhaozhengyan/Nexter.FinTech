
using System;

namespace FinTech.Domain
{
    public class Transaction : IAggregateRoot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memo">备注</param>
        /// <param name="categoryId"></param>
        /// <param name="accountId">支付宝帐户ID</param>
        /// <param name="memberId">会员ID</param>
        /// <param name="bookId">账本</param>
        /// <param name="spending">花销</param>
        /// <param name="income">收入</param>
        /// <param name="createdAt"></param>
        public Transaction(string memo, long categoryId, long accountId, long memberId, long bookId, decimal? spending, decimal? income, DateTime createdAt)
        {
            Memo = memo;
            AccountId = accountId;
            MemberId = memberId;
            BookId = bookId;
            CategoryId = categoryId;
            Spending = spending;
            Income = income;
            Date = createdAt.Date;
            CreatedAt = DateTime.Now;
            LastModifiedAt = DateTime.Now;
        }

        public long Id { get; set; } // Id (Primary key)
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
        public System.DateTime Date { get; set; } // Date
        public System.DateTime CreatedAt { get; set; } // CreatedAt
        public System.DateTime LastModifiedAt { get; set; } // LastModifiedAt
    }

}
