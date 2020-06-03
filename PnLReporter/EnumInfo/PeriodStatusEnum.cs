using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class PeriodStatusEnum
    {
        private Dictionary<int?, string> dictionary;

        public PeriodStatusEnum()
        {
            dictionary = new Dictionary<int?, string>()
            {
                {0, "Đã tạo" },
                {1, "Đang mở" },
                {2, "Đóng" },
                {3, "Mở lại" },
                {4, "Đóng nhưng đã chỉnh sửa" }
            };
        }

        public string getStatus(int? statusId)
        {
            string result;
            dictionary.TryGetValue(statusId, out result);
            return result;
        }
    }
}
