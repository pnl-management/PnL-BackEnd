using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class TransactionCategory
    {
        public TransactionCategory()
        {
            Transaction = new HashSet<Transaction>();
        }

        public decimal CategoryId { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public bool? Status { get; set; }
        public string BrandId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
