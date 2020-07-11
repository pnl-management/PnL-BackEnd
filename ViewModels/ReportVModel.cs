using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class ReportVModel
    {
        public String Id {
            get
            {
                if (Period != null && Store != null)
                {
                    return Period.Id + " - " + Store.Id;
                }
                return null;
            }
        }
        public AccountingPeriodVModel Period { get; set; }
        public StoreVModel Store { get; set; }
        public IEnumerable<TransactionVModel> ListTransactions { get; set; }
    }
}
