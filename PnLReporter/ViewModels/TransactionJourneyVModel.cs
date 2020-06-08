using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class TransactionJourneyVModel
    {
        public long? Id { get; set; }
        public int? Status { get; set; }
        public ParticipantVModel CreatedByParticipant { get; set; }
        public string FeedBack { get; set; }
        public DateTime? CreatedTime { get; set; }
        public TransactionVModel Transaction { get; set; }
    }
}
