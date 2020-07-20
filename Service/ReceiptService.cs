using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;
using PnLReporter.ViewModels;
using PnLReporter.EnumInfo;

namespace PnLReporter.Service
{
    public interface IReceiptService
    {
        IEnumerable<ReceiptVModel> GetReceiptByBrand(int brandId);
        IEnumerable<ReceiptVModel> GetReceiptByStore(int storeId);
        ReceiptVModel GetById(long id);
        ReceiptVModel Create(ReceiptVModel receipt);
        ReceiptVModel Update(ReceiptVModel receipt);
        bool IsReceiptCanModified(long receiptId);
        bool JudgeReceipt(long id, String type);
    }
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _repository;

        public ReceiptService(PLSystemContext context)
        {
            _repository = new ReceiptRepository(context);
        }

        public ReceiptVModel Create(ReceiptVModel receipt)
        {
            var model = new Receipt()
            {
                Name = receipt.Name,
                Value = receipt.Value,
                Description = receipt.Description,
                Status = receipt.Status,
                CategoryId = receipt.Category?.Id,
                BrandId = receipt.Brand.Id,
                StoreId = receipt.Store.Id,
                CreatedTime = DateTime.UtcNow.AddHours(7),
                CreatedBy = receipt.CreateBy.Id
            };
            model = _repository.Create(model);

            if (model == null) return null;

            return ReceiptVModel.ToVModel(model);
        }

        public ReceiptVModel GetById(long id)
        {
            var result = _repository.GetById(id);
            if (result == null) return null;

            return ReceiptVModel.ToVModel(result);
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

        public bool IsReceiptCanModified(long receiptId)
        {
            var current = this.GetById(receiptId);

            if (current == null) return false;

            var listStt = new List<int>()
            {
                ReceiptStatusConst.CREATED,
                ReceiptStatusConst.MODIFIED
            };

            if (listStt.Contains(current.Status ?? -100))
            {
                return true;
            }

            return false;
        }

        public bool JudgeReceipt(long id, string type)
        {
            var current = this.GetById(id);
            if (current == null) return false;
            Receipt result;
            switch (type)
            {
                case TransactionJourneyReqType.APPROVE:
                    current.Status = ReceiptStatusConst.ACC_APPROVED;
                    result = _repository.Update(current);
                    if (result != null) return true;
                    return false;
                case TransactionJourneyReqType.REJECT:
                    current.Status = ReceiptStatusConst.ACC_REJECT;
                    result = _repository.Update(current);
                    if (result != null) return true;
                    return false;
            }

            return false;
        }

        public ReceiptVModel Update(ReceiptVModel receipt)
        {
            receipt.Status = ReceiptStatusConst.MODIFIED;
            var result = _repository.Update(receipt);

            if (result == null) return null;

            return ReceiptVModel.ToVModel(result);
        }
    }
}
