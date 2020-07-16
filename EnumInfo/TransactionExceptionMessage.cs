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
        public const String TRANSACTION_CATEGORY_IS_NULL = "Transaction Category is not null";
        public const String CUR_STATUS_NOT_BE_JUDGE = "Current status of transaction cannot be judged";
        public const String REQ_TYPE_INVALID = "Request type of judge is invalid";
        public const String CURRENT_STATUS_CANNOT_TO_PERIOD = "Current status of transaction is not ready to put to period";
        public const String PERIOD_NOT_OPEN = "Current period is not available to put transaction in";
        public const String PERIOD_STATUS_NOT_APPLY = "Current status cannot be apply to this period";
        public const String CURRENT_TIME_IS_AFTER_DEADLINE = "Current time is after deadline. Cannot open this period";
    }
}
