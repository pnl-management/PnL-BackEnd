using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Repository
{
    public interface IAccountPeriodRepository
    {

    }
    public class AccountingPeriodRepository : IAccountPeriodRepository
    {
        private readonly PLSystemContext _context;

        public AccountingPeriodRepository(PLSystemContext context)
        {
            _context = context;
        }
    }
}
