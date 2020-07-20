using PnLReporter.Models;
using PnLReporter.Repository;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Service
{
    public interface IEvidenceService
    {
        IEnumerable<EvidenceVModel> GetListEvidenceOfReceipt(long receiptId);
        EvidenceVModel GetById(long evidenceId);
        EvidenceVModel UpdateEvidence(EvidenceVModel evidence);
        IEnumerable<EvidenceVModel> InsertEvidences(List<EvidenceVModel> evidencesLst);
        EvidenceVModel DeleteEvidence(long id);
    }
    public class EvidenceService : IEvidenceService
    {
        private readonly IEvidenceRepository _repository;

        public EvidenceService(PLSystemContext context)
        {
            _repository = new EvidenceRepository(context);
        }

        public EvidenceVModel DeleteEvidence(long id)
        {
            var result = _repository.DeleteEvidence(id);
            if (result == null) return null;
            return this.ParseToVModel(new List<Evidence>() { result }).FirstOrDefault();
        }

        public EvidenceVModel GetById(long evidenceId)
        {
            var result = _repository.GetById(evidenceId);
            if (result == null) return null;
            return this.ParseToVModel(new List<Evidence>() { result }).FirstOrDefault();
        }

        public IEnumerable<EvidenceVModel> GetListEvidenceOfReceipt(long receiptId)
        {
            return this.ParseToVModel(_repository.GetListEvidenceOfReceipt(receiptId));
        }

        public IEnumerable<EvidenceVModel> InsertEvidences(List<EvidenceVModel> evidencesLst)
        {
            var modelLst = new List<Evidence>();

            foreach (var evidence in evidencesLst)
            {
                var model = new Evidence()
                {
                    Title = evidence.Title,
                    Description = evidence.Description,
                    Url = evidence.Url,
                    ReceiptId = evidence.Receipt.Id
                };
                modelLst.Add(model);
            }

            return this.ParseToVModel(_repository.InsertEvidences(modelLst));
        }

        public EvidenceVModel UpdateEvidence(EvidenceVModel evidence)
        {
            var result = _repository.UpdateEvidence(evidence);

            if (result == null) return null;

            return this.ParseToVModel(new List<Evidence>() { result }).FirstOrDefault();
        }

        private IEnumerable<EvidenceVModel> ParseToVModel(IEnumerable<Evidence> listModel)
        {
            var result = new List<EvidenceVModel>();

            foreach (var model in listModel)
            {
                var vmodel = EvidenceVModel.ToVModel(model);
                result.Add(vmodel);
            }

            return result;
        }
    }
}
