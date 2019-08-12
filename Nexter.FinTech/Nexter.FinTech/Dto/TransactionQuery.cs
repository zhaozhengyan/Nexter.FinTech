using System;

namespace Nexter.FinTech
{
    public class TransactionQuery
    {
        public long AccountId { get; set; }
        public long CategoryId { get; set; }
        public DateTime? Date { get; set; }
        public int Take => int.MaxValue;
        public int Skip => 0;
    }
}