using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class TransactionStatusEnum
    {
        private readonly Dictionary<int?, string> Dictionary;

        public readonly static int STORE_CREATED = 201;
        public readonly static int STORE_MODIFIED = 202;
        public readonly static int STORE_CANCELED = 203;

        public readonly static int ACC_APPROVED = 301;
        public readonly static int ACC_REQ_MODIFIED = 302;
        public readonly static int ACC_CANCELED = 203;

        public readonly static int INVESTOR_APPROVED = 201;
        public readonly static int INVESTOR_REQ_MODIFIED = 202;
        public readonly static int INVESTOR_CANCEL = 203;

        public readonly static int DONE = 1;
        public readonly static int CANCELED_AFTER_CLOSED = 0;

        public TransactionStatusEnum()
        {
            Dictionary = new Dictionary<int?, string>()
            {
                { STORE_CREATED, "Cửa hàng đã tạo"},
                { STORE_MODIFIED, "Cửa hàng đã chỉnh sửa" },
                { STORE_CANCELED, "Cửa hàng đã hủy" },

                { ACC_APPROVED, "Kế toán chấp thuận" },
                { ACC_REQ_MODIFIED, "Kế toán yêu cầu chỉnh sửa" },
                { ACC_CANCELED, "Kế toán đã hủy" },

                { INVESTOR_APPROVED, "Chủ đầu tư chấp thuận" },
                { INVESTOR_REQ_MODIFIED, "Chủ đầu tư yêu cầu chỉnh sửa"},
                { INVESTOR_CANCEL, "Chủ đầu tư đã hủy" },

                { DONE, "Hoàn tất" },
                { CANCELED_AFTER_CLOSED, "Bị hủy sau khi quyết toán" }
            };
        }

        public string GetStatus(int? statusId)
        {
            string value = "";
            Dictionary.TryGetValue(statusId, out value);

            return value; 
        }
    }
}
