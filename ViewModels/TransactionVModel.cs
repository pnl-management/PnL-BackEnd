using PnLReporter.Models;
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
        public TransactionCategoryVModel Category { get; set; }
        public AccountingPeriodVModel Period { get; set; }
        public BrandVModel Brand { get; set; }
        public StoreVModel Store { get; set; }
        public DateTime? CreatedTime { get; set; }
        public ParticipantVModel CreateBy { get; set; }
        public TransactionJourneyVModel LastestStatus { get; set; }

        public List<ReceiptVModel> ListReceipt { get; set; }

        public static TransactionVModel ToVModel(Transaction model)
        {
            if (model == null) return null;

            var vmodel = new TransactionVModel()
            {
                Id = model.Id,
                Name = model.Name,
                Value = model.Value,
                Description = model.Description,
                Category = TransactionCategoryVModel.ToVModel(model.Category) ?? new TransactionCategoryVModel() { Id = model.CategoryId},
                Period = AccountingPeriodVModel.ToVModel(model.Period) ?? new AccountingPeriodVModel() { Id = model.PeriodId},
                Brand = BrandVModel.ToVModel(model.Brand) ?? new BrandVModel() { Id = model.BrandId},
                Store = StoreVModel.ToVModel(model.Store) ?? new StoreVModel() { Id = model.StoreId},
                CreatedTime = model.CreatedTime,
                CreateBy = ParticipantVModel.ToVModel(model.CreatedByNavigation) ?? new ParticipantVModel() { Id = model.CreatedBy}
            };

            return vmodel;
        }
    }
}
