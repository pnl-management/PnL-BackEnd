using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class TransactionJourney
    {
        public long Id { get; set; }
        public int? Status { get; set; }
        public int? CreatedBy { get; set; }
        public string FeedBack { get; set; }
        public DateTime? CreatedTime { get; set; }
        public long? TransactionId { get; set; }

        public virtual Participant CreatedByNavigation { get; set; }
        public virtual Transaction Transaction { get; set; }
    }
}
