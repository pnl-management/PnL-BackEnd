using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class TransactionJourneyReqType
    {
        public const String APPROVE = "approve";
        public const String REJECT = "reject";
        public const String REQ_MODIFIED = "req-modified";
        public const String CANCELED_AFTER_CLOSE = "cancel-after-close";
    }
}
