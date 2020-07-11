using Microsoft.EntityFrameworkCore;
using PnLReporter.EnumInfo;
using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Repository
{
    public interface IReportRepository
    {
        IEnumerable<Transaction> GetListTransactionOfStoreAndPeriod(long storeId, int periodId);
        IEnumerable<AccountingPeriod> GetListClosedPeriod(int brandId);
        IEnumerable<Store> GetListStoreOfBrand(int brandId);
    }
    public class ReportRepository : IReportRepository
    {
        private readonly PLSystemContext _context;

        public ReportRepository(PLSystemContext context)
        {
            _context = context;
        }

        public IEnumerable<AccountingPeriod> GetListClosedPeriod(int brandId)
        {
            var listCloseStatus = new List<int>()
            {
                PeriodStatusConst.RE_OPEN,
                PeriodStatusConst.CLOSED,
                PeriodStatusConst.CLOSE_BUT_MODIFIED
            };

            return _context.AccountingPeriod
                .Where(record =>
                    record.BrandId == brandId &&
                    listCloseStatus.Contains(record.Status ?? -100)
                )
                .OrderBy(record => record.StartDate)
                .ToList();
        }

        public IEnumerable<Store> GetListStoreOfBrand(int brandId)
        {
            return _context.Store
                .Where(record => record.BrandId == brandId)
                .OrderBy(record => record.Name)
                .ToList();
        }

        public IEnumerable<Transaction> GetListTransactionOfStoreAndPeriod(long storeId, int periodId)
        {
            return _context.Transaction
                .Include(record => record.Category)
                .Include(record => record.Store)
                .Include(record => record.Period)
                .Include(record => record.CreatedByNavigation)
                .Where(record =>
                    record.PeriodId == periodId
                    && record.StoreId == storeId
                )
                .OrderByDescending(record => record.CreatedTime)
                .ToList();
        }
    }
}
