using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class ReceiptStatusConst
    {
        public const int ACC_REJECT = -2;
        public const int CANCEL = -1;
        public const int CREATED = 0;
        public const int MODIFIED = 1;
        public const int ACC_APPROVED = 2;
    }
}
