using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PnLReporter.Models
{
    public partial class TransactionCategory
    {
        public TransactionCategory()
        {
            Transaction = new HashSet<Transaction>();
        }
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }
        public int? Type { get; set; }
        public bool? Required { get; set; }
        public bool? Status { get; set; }
        public int? BrandId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
