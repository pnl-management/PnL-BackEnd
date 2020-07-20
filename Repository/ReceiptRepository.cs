using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.EnumInfo;
using PnLReporter.Models;
using PnLReporter.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace PnLReporter.Repository
{
    public interface IReceiptRepository
    {
        Receipt Create(Receipt model);
        Receipt Update(ReceiptVModel vmodel);
        IEnumerable<Receipt> GetListByBrand(int brandId);
        IEnumerable<Receipt> GetListByStore(int storeId);
        Receipt GetById(long id);
    }
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly PLSystemContext _context;

        public ReceiptRepository(PLSystemContext context)
        {
            this._context = context;
        }

        public Receipt Create(Receipt model)
        {
            _context.Receipt.Add(model);
            _context.SaveChanges();
            return model;
        }

        public IEnumerable<Receipt> GetListByBrand(int brandId)
        {
            var result = _context.Receipt
                .Include(record => record.Brand)
                .Include(record => record.Store)
                .Include(record => record.Category)
                .Include(record => record.CreatedByNavigation)
                .Include(record => record.LastModifiedByNavigation)
                .Where(record => record.BrandId == brandId)
                .ToList();

            return result;
        }

        public Receipt GetById(long id)
        {
            return _context.Receipt.Find(id);
        }

        public IEnumerable<Receipt> GetListByStore(int storeId)
        {
            var result = _context.Receipt
                .Include(record => record.Brand)
                .Include(record => record.Store)
                .Include(record => record.Category)
                .Include(record => record.CreatedByNavigation)
                .Include(record => record.LastModifiedByNavigation)
                .Where(record => record.StoreId == storeId)
                .ToList();

            return result;
        }

        public Receipt Update(ReceiptVModel vmodel)
        {
            var model = _context.Receipt.Find(vmodel.Id);
            if (model == null) return null;

            model.Name = vmodel.Name;
            model.Value = vmodel.Value;
            model.Description = vmodel.Description;
            model.Status = vmodel.Status;
            model.CategoryId = vmodel.Category != null ? vmodel.Category.Id : null;
            model.BrandId = vmodel.Brand.Id;
            model.StoreId = vmodel.Store.Id;
            model.LastModified = DateTime.UtcNow.AddHours(7);
            model.LastModifiedBy = vmodel.LastModifiedBy.Id;

            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();

            return model;
        }
    }
}
