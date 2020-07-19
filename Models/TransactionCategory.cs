using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class TransactionCategory
    {
        public TransactionCategory()
        {
            Receipt = new HashSet<Receipt>();
            Transaction = new HashSet<Transaction>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public bool? Required { get; set; }
        public bool? Status { get; set; }
        public int? BrandId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Receipt> Receipt { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
