using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class EvidenceVModel
    {
        public long? Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public TransactionVModel Transaction { get; set; }
        public string Description { get; set; }
    }
}
