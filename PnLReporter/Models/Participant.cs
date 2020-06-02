using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Participant
    {
        public Participant()
        {
            Transaction = new HashSet<Transaction>();
        }

        public string Username { get; set; }
        public string Fullname { get; set; }
        public int? Role { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string BrandId { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
