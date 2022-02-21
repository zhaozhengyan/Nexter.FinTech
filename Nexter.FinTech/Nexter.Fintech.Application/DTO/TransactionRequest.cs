using System;

namespace Nexter.Fintech.Application
{
    public class TransactionRequest
    {
        public long AccountId { get; set; }
        public long CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Money { get; set; }
        public string Memo { get; set; }
    }
}