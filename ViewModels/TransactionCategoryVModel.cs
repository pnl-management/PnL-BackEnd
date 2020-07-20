using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class TransactionCategoryVModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public bool? Required { get; set; }
        public bool? Status { get; set; }
        public BrandVModel Brand { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastModified { get; set; }

        public static TransactionCategoryVModel ToVModel(TransactionCategory model)
        {
            if (model == null) return null;

            var vmodel = new TransactionCategoryVModel()
            {
                Id = model.Id,
                Name = model.Name,
                Type = model.Type,
                Required = model.Required,
                Status = model.Status,
                Brand = BrandVModel.ToVModel(model.Brand) ?? new BrandVModel() { Id = model.BrandId},
                CreatedTime = model.CreatedTime,
                LastModified = model.LastModified
            };

            return vmodel;
        }
    }
}
