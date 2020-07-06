using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Repository
{
    public interface IPeriodRepository
    {
        AccountingPeriod GetById(long id);
    }
    public class PeriodRepository : IPeriodRepository
    {
        private readonly PLSystemContext _context;

        public PeriodRepository(PLSystemContext context)
        {
            _context = context;
        }

        public AccountingPeriod GetById(long id)
        {
            return _context.AccountingPeriod
                .AsNoTracking()
                .Where(record => record.Id == id)
                .FirstOrDefault();
        }
    }
}
