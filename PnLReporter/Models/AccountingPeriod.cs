using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class AccountingPeriod
    {
        public AccountingPeriod()
        {
            Transaction = new HashSet<Transaction>();
        }

        public decimal PeriodId { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CreateTime { get; set; }
        public string BrandId { get; set; }
        public DateTime? LastModifed { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
