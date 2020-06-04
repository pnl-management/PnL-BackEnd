using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class TransactionCategoryVModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public bool? Required { get; set; }
        public bool? Status { get; set; }
        public int? BrandId { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
