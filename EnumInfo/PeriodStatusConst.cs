using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class PeriodStatusConst
    {
        private Dictionary<int?, string> Dictionary;

        public const int CREATED = 0;
        public const int OPENING = 1;
        public const int CLOSED = 2;
        public const int RE_OPEN = 3;
        public const int CLOSE_BUT_MODIFIED = 4;

        public PeriodStatusConst()
        {
            Dictionary = new Dictionary<int?, string>()
            {
                {CREATED, "Đã tạo" },
                {OPENING, "Đang mở" },
                {CLOSED, "Đóng" },
                {RE_OPEN, "Mở lại" },
                {CLOSE_BUT_MODIFIED, "Đóng nhưng đã chỉnh sửa" }
            };
        }

        public string GetStatus(int? statusId)
        {
            string result;
            Dictionary.TryGetValue(statusId, out result);
            return result;
        }
    }
}
