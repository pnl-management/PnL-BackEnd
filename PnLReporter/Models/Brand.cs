using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Models
{
    public class Brand
    {
        public string BrandId { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
