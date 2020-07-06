using PnLReporter.EnumInfo;
using PnLReporter.Models;
using PnLReporter.Repository;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Service
{
    public interface IAccountingPeriodService
    {
        IEnumerable<AccountingPeriodVModel> GetListByBrand(int brandId);
        AccountingPeriodVModel GetById(int id);
        AccountingPeriodVModel Update(AccountingPeriodVModel period);
        AccountingPeriodVModel Insert(AccountingPeriodVModel period);
    }
    public class AccountingPeriodService : IAccountingPeriodService
    {
        private readonly IAccountPeriodRepository _repository;

        public AccountingPeriodService(PLSystemContext context)
        {
            _repository = new AccountingPeriodRepository(context);
        }

        public AccountingPeriodVModel GetById(int id)
        {
            var result = _repository.GetById(id);
            if (result == null) return null;
            return this.ParseToVModel(new List<AccountingPeriod>() { result }).FirstOrDefault();
        }

        public IEnumerable<AccountingPeriodVModel> GetListByBrand(int brandId)
        {
            return this.ParseToVModel(_repository.GetListByBrand(brandId));
        }

        public AccountingPeriodVModel Insert(AccountingPeriodVModel period)
        {
            var model = new AccountingPeriod()
            {
                BrandId = period.Brand.Id,
                Title = period.Title,
                Status = PeriodStatusConst.CREATED,
                CreateTime = DateTime.Now,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Deadline = period.Deadline
            };

            var result = _repository.Insert(model);

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
                    Brand = model.Brand != null ? new BrandVModel()
                    {
                        Id = model.Brand.Id,
                        Name = model.Brand.Name
                    },
                    CreateTime = model.CreateTime,
                    Deadline = model.Deadline,
                    EndDate = model.EndDate,
                    StartDate = model.StartDate,
                    Status = model.Status,
                    Title = model.Title
                };
                result.Add(vmodel);
            }
            return result;
        }

        public AccountingPeriodVModel Update(AccountingPeriodVModel period)
        {
            var result = _repository.Update(period);
            if (result == null) return null;
            return this.ParseToVModel(new List<AccountingPeriod>() { result }).FirstOrDefault();
        }
    }
}
