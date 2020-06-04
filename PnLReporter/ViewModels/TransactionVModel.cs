using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class TransactionVModel
    {
        public TransactionVModel()
        {
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public long? CategoryId { get; set; }
        public TransactionCategoryVModel Category { get; set; }
        public int? PeriodId { get; set; }
        public AccountingPeriodVModel Period { get; set; }
        public int? BrandId { get; set; }
        public BrandVModel Brand { get; set; }
        public int? StoreId { get; set; }
        public StoreVModel Store { get; set; }
        public DateTime? CreatedTime { get; set; }
        public int? CreatedById { get; set; }
        public ParticipantVModel CreateByParticipant { get; set; }
    }
}
