using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class BrandVModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedTime { get; set; }

        public static BrandVModel ToVModel(Brand model)
        {
            if (model == null) return null;

            var vmodel = new BrandVModel()
            {
                Id = model.Id,
                Name = model.Name,
                Status = model.Status,
                CreatedTime = model.CreatedTime
            };

            return vmodel;
        }
    }
}
