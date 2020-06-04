using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class AccountingPeriodVModel
    {
        public int Id { get; set; }
        public int? Status { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CreateTime { get; set; }
        public int? BrandId { get; set; }
        public BrandVModel Brand { get; set; }
        public DateTime? LastModifed { get; set; }
    }
}
