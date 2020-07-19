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
        bool Delete(int id);
        AccountingPeriodVModel ChangeStatus(AccountingPeriodVModel period);
    }
    public class AccountingPeriodService : IAccountingPeriodService
    {
        private readonly IAccountPeriodRepository _repository;

        public AccountingPeriodService(PLSystemContext context)
        {
            _repository = new AccountingPeriodRepository(context);
        }

        public AccountingPeriodVModel ChangeStatus(AccountingPeriodVModel period)
        {
            var pervious = this.GetById(period.Id ?? -10);
            var currentStatus = pervious.Status ?? -10;
            var newStatus = period.Status ?? -10;
            AccountingPeriodVModel result = null;

            switch (newStatus)
            {
                case PeriodStatusConst.OPENING:

                    if (currentStatus == PeriodStatusConst.CREATED)
                    {
                        var check = _repository.ChangeStatus(period);
                        if (check == null)
                        {
                            result = null;
                        }
                        else
                        {
                            result = this.ParseToVModel(new List<AccountingPeriod>() { check }).FirstOrDefault();
                        }
                    }
                    else
                    {
                        throw new Exception(TransactionExceptionMessage.CURRENT_STATUS_CANNOT_TO_PERIOD);
                    }
                    break;
                case PeriodStatusConst.RE_OPEN:
                    var listStatusAble = new List<int>()
                    {
                        PeriodStatusConst.CLOSED,
                        PeriodStatusConst.CLOSE_BUT_MODIFIED
                    };

                    if (listStatusAble.Contains(currentStatus))
                    {
                        var check = _repository.ChangeStatus(period);
                        if (check == null)
                        {
                            result = null;
                        }
                        else
                        {
                            result = this.ParseToVModel(new List<AccountingPeriod>() { check }).FirstOrDefault();
                        }
                    }
                    else
                    {
                        throw new Exception(TransactionExceptionMessage.CURRENT_STATUS_CANNOT_TO_PERIOD);
                    }
                    break;
                case PeriodStatusConst.CLOSE_BUT_MODIFIED:
                    if (currentStatus == PeriodStatusConst.RE_OPEN)
                    {
                        var check = _repository.ChangeStatus(period);
                        if (check == null)
                        {
                            result = null;
                        }
                        else
                        {
                            result = this.ParseToVModel(new List<AccountingPeriod>() { check }).FirstOrDefault();
                        }
                    }
                    else
                    {
                        throw new Exception(TransactionExceptionMessage.CURRENT_STATUS_CANNOT_TO_PERIOD);
                    }
                    break;
                case PeriodStatusConst.CLOSED:
                    if (currentStatus == PeriodStatusConst.OPENING)
                    {
                        var check = _repository.ChangeStatus(period);
                        if (check == null)
                        {
                            result = null;
                        }
                        else
                        {
                            result = this.ParseToVModel(new List<AccountingPeriod>() { check }).FirstOrDefault();
                        }
                    }
                    else
                    {
                        throw new Exception(TransactionExceptionMessage.CURRENT_STATUS_CANNOT_TO_PERIOD);
                    }
                    break;
            }
            return result;
        }

        public bool Delete(int id)
        {
            return _repository.Delete(id);
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
                CreateTime = DateTime.UtcNow.AddHours(7),
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
                var vmodel = AccountingPeriodVModel.ToVModel(model);
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
