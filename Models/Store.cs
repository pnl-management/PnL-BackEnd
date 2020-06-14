using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Store
    {
        public Store()
        {
            StoreParticipantsDetail = new HashSet<StoreParticipantsDetail>();
            Transaction = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool? Status { get; set; }
        public int? BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<StoreParticipantsDetail> StoreParticipantsDetail { get; set; }
        public virtual ICollection<Transaction> Transaction { get; set; }
    }
}
