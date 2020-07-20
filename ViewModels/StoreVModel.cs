using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;

namespace PnLReporter.ViewModels
{
    public class StoreVModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool? Status { get; set; }
        public BrandVModel Brand { get; set; }

        public static StoreVModel ToVModel(Store model)
        {
            if (model == null) return null;

            var vmodel = new StoreVModel()
            {
                Id = model.Id,
                Name = model.Name,
                Phone = model.Phone,
                Address = model.Address,
                Status = model.Status,
                Brand = BrandVModel.ToVModel(model.Brand) ?? new BrandVModel() { Id = model.BrandId}
            };

            return vmodel;
        }
    }
}
