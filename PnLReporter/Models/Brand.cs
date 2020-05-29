using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Brand
    {
        public Brand()
        {
            AccountingPeriod = new HashSet<AccountingPeriod>();
            Participant = new HashSet<Participant>();
            TransactionCategory = new HashSet<TransactionCategory>();
        }

        public string BrandId { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedTime { get; set; }

        public virtual ICollection<AccountingPeriod> AccountingPeriod { get; set; }
        public virtual ICollection<Participant> Participant { get; set; }
        public virtual ICollection<TransactionCategory> TransactionCategory { get; set; }
    }
}
