using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class TransactionStatusConst
    {
        public readonly static int ACC_CREATE = 301;
        public readonly static int ACC_MODIFIED = 302;
        public readonly static int ACC_CANCELED = 303;

        public readonly static int INVESTOR_APPROVED = 101;
        public readonly static int INVESTOR_REQ_MODIFIED = 102;
        public readonly static int INVESTOR_CANCEL = 103;

        public readonly static int DONE = 1;
        public readonly static int CANCELED_AFTER_CLOSED = 0;
    }
}
