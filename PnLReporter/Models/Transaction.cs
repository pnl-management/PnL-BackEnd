using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            Evidence = new HashSet<Evidence>();
            TransactionJorney = new HashSet<TransactionJorney>();
        }

        public string TransactionId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string MasterTransactionId { get; set; }
        public decimal? CategoryId { get; set; }
        public decimal? PeriodId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string CreatedBy { get; set; }

        public virtual TransactionCategory Category { get; set; }
        public virtual Participant CreatedByNavigation { get; set; }
        public virtual AccountingPeriod Period { get; set; }
        public virtual ICollection<Evidence> Evidence { get; set; }
        public virtual ICollection<TransactionJorney> TransactionJorney { get; set; }
    }
}
