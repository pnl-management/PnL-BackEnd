using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class Evidence
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public long? TransactionId { get; set; }
        public string Description { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}
