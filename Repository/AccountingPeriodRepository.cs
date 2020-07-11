using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Repository
{
    public interface IAccountPeriodRepository
    {
        IEnumerable<AccountingPeriod> GetListByBrand(int brandId);
        AccountingPeriod GetById(int id);
        AccountingPeriod Update(AccountingPeriodVModel period);
        AccountingPeriod Insert(AccountingPeriod period);
        bool Delete(int id);
    }
    public class AccountingPeriodRepository : IAccountPeriodRepository
    {
        private readonly PLSystemContext _context;

        public AccountingPeriodRepository(PLSystemContext context)
        {
            _context = context;
        }

        public bool Delete(int id)
        {
            var current = this.GetById(id);

            if (current == null) return false;

            _context.AccountingPeriod.Remove(current);
            _context.SaveChanges();

            return true;
        }

        public AccountingPeriod GetById(int id)
        {
            return _context.AccountingPeriod
                .Include(record => record.Transaction)
                .Where(record => record.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<AccountingPeriod> GetListByBrand(int brandId)
        {
            return _context.AccountingPeriod
                .Include(record => record.Transaction)
                .Where(record => record.BrandId == brandId)
                .ToList();
        }

        public AccountingPeriod Insert(AccountingPeriod period)
        {
            _context.AccountingPeriod.Add(period);
            _context.SaveChanges();

            return period;
        }

        public AccountingPeriod Update(AccountingPeriodVModel period)
        {
            var model = this.GetById(period.Id ?? default);

            if (model == null) return null;

            model.LastModifed = DateTime.UtcNow.AddHours(7);
            model.StartDate = period.StartDate;
            model.Status = period.Status;
            model.Title = period.Title;
            model.Deadline = period.Deadline;
            model.EndDate = period.EndDate;

            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();

            return model;
        }
    }
}
