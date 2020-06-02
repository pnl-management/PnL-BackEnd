using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class TransactionJorney
    {
        public string JorneyId { get; set; }
        public int? Status { get; set; }
        public string CreatedBy { get; set; }
        public string FeedBack { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string TransactionId { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
