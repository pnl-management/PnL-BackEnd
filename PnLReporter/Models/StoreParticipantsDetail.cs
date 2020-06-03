using System;
using System.Collections.Generic;

namespace PnLReporter.Models
{
    public partial class StoreParticipantsDetail
    {
        public int StoreId { get; set; }
        public int ParticipantId { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual Participant Participant { get; set; }
        public virtual Store Store { get; set; }
    }
}
