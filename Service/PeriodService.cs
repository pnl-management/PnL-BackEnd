using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using PnLReporter.Repository;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Service
{
    public interface IPeriodService
    {
        AccountingPeriodVModel GetById(long id);
    }
    public class PeriodService : IPeriodService
    {
        private readonly IPeriodRepository _repository;

        public PeriodService(PLSystemContext context)
        {
            _repository = new PeriodRepository(context);
        }

        public AccountingPeriodVModel GetById(long id)
        {
            var result = _repository.GetById(id);
            if (result == null) return null;
            return this.ParseToVModel(new List<AccountingPeriod>() { result }).FirstOrDefault();
        }

        public IEnumerable<AccountingPeriodVModel> ParseToVModel(IEnumerable<AccountingPeriod> listModel)
        {
            var result = new List<AccountingPeriodVModel>();
            foreach (var model in listModel)
            {
                var vmodel = new AccountingPeriodVModel()
                {
                    Id = model.Id,
                    Title = model.Title,
                    Status = model.Status,
                    Brand = model.Brand != null ? new BrandVModel()
                    {
                        Id = model.Brand.Id,
                        Name = model.Brand.Name
                    } : null,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Deadline = model.Deadline,
                    CreateTime = model.CreateTime,
                    LastModifed = model.LastModifed
                };

                result.Add(vmodel);
            }
            return result;
        }
    }
}
