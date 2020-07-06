using PnLReporter.Models;
using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Service
{
    public interface IAccountingPeriodService
    {

    }
    public class AccountingPeriodService : IAccountingPeriodService
    {
        private readonly IAccountPeriodRepository _repository;

        public AccountingPeriodService(PLSystemContext context)
        {
            _repository = new AccountingPeriodRepository(context);
        }
    }
}
