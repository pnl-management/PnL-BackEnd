using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.EnumInfo
{
    public class TransactionStatusEnum
    {
        private readonly Dictionary<int?, string> dictionary;

        public TransactionStatusEnum()
        {
            dictionary = new Dictionary<int?, string>()
            {
                { 201, "Đã tạo"},
                { 202, "Đã chỉnh sửa" },

                { 301, "Kế toán đã hủy" },
                { 302, "Kế toán yêu cầu chỉnh sửa" },
                { 303, "Kế toán chấp thuận" },

                { 101, "Chủ đầu tư đã hủy" },
                { 102, "Chủ đầu tư yêu cầu chỉnh sửa"},
                { 103, "Chủ đầu tư chấp thuận" },

                { 1, "Hoàn tất" },
                { 0, "Hủy sau khi quyết toán" }
            };
        }

        public string GetStatus(int? statusId)
        {
            string value = "";
            dictionary.TryGetValue(statusId, out value);

            return value; 
        }
    }
}
