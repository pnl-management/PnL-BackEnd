using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
            Evidence = new HashSet<Evidence>();
            TransactionJourney = new HashSet<TransactionJourney>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public long? CategoryId { get; set; }
        public int? PeriodId { get; set; }
        public int? BrandId { get; set; }
        public int? StoreId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public int? CreatedBy { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual TransactionCategory Category { get; set; }
        public virtual Participant CreatedByNavigation { get; set; }
        public virtual AccountingPeriod Period { get; set; }
        public virtual Store Store { get; set; }
        public virtual ICollection<Evidence> Evidence { get; set; }
        public virtual ICollection<TransactionJourney> TransactionJourney { get; set; }
    }
}
