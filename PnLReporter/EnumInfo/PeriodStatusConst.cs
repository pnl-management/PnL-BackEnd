using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class PeriodStatusConst
    {
        private Dictionary<int?, string> Dictionary;

        public static readonly int CREATED = 0;
        public static readonly int OPENING = 1;
        public static readonly int CLOSED = 2;
        public static readonly int RE_OPEN = 3;
        public static readonly int CLOSE_BUT_MODIFIED = 4;

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
