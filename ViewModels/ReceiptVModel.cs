using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class ReceiptVModel
    {
        public ReceiptVModel()
        {
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public TransactionCategoryVModel Category { get; set; }
        public BrandVModel Brand { get; set; }
        public StoreVModel Store { get; set; }
        public DateTime? CreatedTime { get; set; }
        public ParticipantVModel CreateBy { get; set; }
        public DateTime? LastModified { get; set; }
        public ParticipantVModel LastModifiedBy { get; set; }
        public int? Status { get; set; }

        public static ReceiptVModel ToVModel(Receipt model)
        {
            if (model == null) return null;

            var vmodel = new ReceiptVModel()
            {
                Id = model.Id,
                Name = model.Name,
                Value = model.Value,
                Description = model.Description,
                Category = TransactionCategoryVModel.ToVModel(model.Category) ?? new TransactionCategoryVModel() {Id = model.CategoryId },
                Brand = BrandVModel.ToVModel(model.Brand) ?? new BrandVModel() { Id = model.BrandId},
                Store = StoreVModel.ToVModel(model.Store) ?? new StoreVModel() { Id = model.StoreId},
                CreatedTime = model.CreatedTime,
                CreateBy = ParticipantVModel.ToVModel(model.CreatedByNavigation) ?? new ParticipantVModel() { Id = model.CreatedBy},
                LastModified = model.LastModified,
                LastModifiedBy = ParticipantVModel.ToVModel(model.LastModifiedByNavigation) ?? new ParticipantVModel() { Id = model.LastModifiedBy },
                Status = model.Status
            };

            return vmodel;
        }
    }
}
