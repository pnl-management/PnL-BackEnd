using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;
using PnLReporter.ViewModels;

namespace PnLReporter.Service
{
    public interface IReceiptService
    {
        IEnumerable<ReceiptVModel> GetReceiptByBrand(int brandId);
        IEnumerable<ReceiptVModel> GetReceiptByStore(int storeId);
    }
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _repository;

        public ReceiptService(PLSystemContext context)
        {
            _repository = new ReceiptRepository(context);
        }

        public IEnumerable<ReceiptVModel> GetReceiptByBrand(int brandId)
        {
            var listModel = _repository.GetListByBrand(brandId);
            if (listModel == null) return null;

            var result = new List<ReceiptVModel>();
            foreach (var model in listModel)
            {
                var vmodel = ReceiptVModel.ToVModel(model);
                if (vmodel != null) result.Add(vmodel);
            }

            return result;
        }

        public IEnumerable<ReceiptVModel> GetReceiptByStore(int storeId)
        {
            var listModel = _repository.GetListByStore(storeId);
            if (listModel == null) return null;

            var result = new List<ReceiptVModel>();
            foreach (var model in listModel)
            {
                var vmodel = ReceiptVModel.ToVModel(model);
                if (vmodel != null) result.Add(vmodel);
            }

            return result;
        }
    }
}
