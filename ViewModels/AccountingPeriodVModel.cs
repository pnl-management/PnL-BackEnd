using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class AccountingPeriodVModel
    {
        public int? Id { get; set; }
        public int? Status { get; set; }
        public string Title { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? CreateTime { get; set; }
        public BrandVModel Brand { get; set; }
        public DateTime? LastModifed { get; set; }

        public static AccountingPeriodVModel ToVModel(AccountingPeriod model)
        {
            if (model == null) return null;

            var vmodel = new AccountingPeriodVModel()
            {
                Id = model.Id,
                Brand = model.Brand != null ? new BrandVModel()
                {
                    Id = model.Brand.Id,
                    Name = model.Brand.Name
                } : new BrandVModel() { Id = model.BrandId },
                CreateTime = model.CreateTime,
                Deadline = model.Deadline,
                EndDate = model.EndDate,
                StartDate = model.StartDate,
                Status = model.Status,
                Title = model.Title,
                LastModifed = model.LastModifed
            };
            return vmodel;
        }
    }
}
