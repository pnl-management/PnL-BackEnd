using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class TransactionExceptionMessage
    {
        public const String OBJ_IS_NULL = "Transaction is null";
        public const String TRANSACTION_NOT_FOUND = "Transaction not found";
        public const String CUR_STATUS_CANNOT_MODIFIED = "Transaction is now cannot be modified";
        public const String CUR_STATUS_NOT_FOUND = "Error data. Current status of transaction cannot be found";
    }
}
