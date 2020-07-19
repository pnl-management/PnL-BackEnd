using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class RecepitTransactionDetail
    {
        public long ReceiptId { get; set; }
        public long TransactionId { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public int? CreatedById { get; set; }
        public DateTime? LastModified { get; set; }
        public int? LastModifiedById { get; set; }

        public virtual Participant CreatedBy { get; set; }
        public virtual Participant LastModifiedBy { get; set; }
        public virtual Receipt Receipt { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
