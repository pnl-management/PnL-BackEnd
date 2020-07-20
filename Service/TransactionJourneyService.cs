using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.ViewModels;
using PnLReporter.Repository;
using PnLReporter.Models;
using PnLReporter.EnumInfo;

namespace PnLReporter.Service
{
    public interface ITransactionJourneyService
    {
        TransactionJourneyVModel GetLastestStatus(long transactionId);
        IEnumerable<TransactionJourneyVModel> GetJourneyOfTransaction(long transactionId);
        TransactionJourneyVModel AddStatus(TransactionJourneyVModel journey);
        TransactionJourneyVModel JudgeTransaction(TransactionJourneyVModel journey, String type, String role);
        TransactionJourneyVModel FindById(long id);
    }
    public class TransactionJourneyService : ITransactionJourneyService
    {
        private readonly ITransactionJourneyRepository _repository;
        private readonly PLSystemContext _context;

        public TransactionJourneyService(PLSystemContext context)
        {
            _context = context;
            _repository = new TransactionJourneyRepository(context);
        }

        public TransactionJourneyVModel AddStatus(TransactionJourneyVModel journey)
        {
            journey.Id = null;
            journey.CreatedTime = DateTime.UtcNow.AddHours(7);

            return ParseToVModel(new List<TransactionJourney> { _repository.AddStatus(journey) }).FirstOrDefault();
        }

        public TransactionJourneyVModel FindById(long id)
        {
            var result = _repository.FindById(id);
            if (result == null) return null;
            return this.ParseToVModel(new List<TransactionJourney>() { result }).FirstOrDefault();
        }

        public IEnumerable<TransactionJourneyVModel> GetJourneyOfTransaction(long transactionId)
        {
            return this.ParseToVModel(_repository.GetJourneyOfTransaction(transactionId));
        }

        public TransactionJourneyVModel GetLastestStatus(long transactionId)
        {
            return _repository.GetLastestStatus(transactionId) != null ? new TransactionJourneyVModel()
            {
                Id = _repository.GetLastestStatus(transactionId).Id,
                Status = _repository.GetLastestStatus(transactionId).Status,
                FeedBack = _repository.GetLastestStatus(transactionId).FeedBack,
                CreatedTime = _repository.GetLastestStatus(transactionId).CreatedTime
            } : null;
        }

        public TransactionJourneyVModel JudgeTransaction(TransactionJourneyVModel journey, String type, String role)
        {
            var lastestStatus = this.GetLastestStatus(journey.Transaction.Id);

            var investorCanJudgeStt = new List<int>()
            {
                TransactionStatusConst.ACC_CREATE,
                TransactionStatusConst.ACC_MODIFIED
            };

            switch (role)
            {
                case ParticipantsRoleConst.INVESTOR:
                    if (investorCanJudgeStt.Contains(lastestStatus.Status ?? default))
                    {
                        switch (type)
                        {
                            case TransactionJourneyReqType.APPROVE:
                                journey.Status = TransactionStatusConst.INVESTOR_APPROVED;
                                break;
                            case TransactionJourneyReqType.REJECT:
                                journey.Status = TransactionStatusConst.INVESTOR_CANCEL;
                                break;
                            case TransactionJourneyReqType.REQ_MODIFIED:
                                journey.Status = TransactionStatusConst.INVESTOR_REQ_MODIFIED;
                                break;
                            default:
                                throw new Exception(TransactionExceptionMessage.REQ_TYPE_INVALID);
                        }
                    }
                    else
                    {
                        throw new Exception(TransactionExceptionMessage.CUR_STATUS_NOT_BE_JUDGE);
                    }
                    break;
            }
            var result = _repository.AddStatus(journey);

            return this.ParseToVModel(new List<TransactionJourney>() { result }).FirstOrDefault();
        }

        private IEnumerable<TransactionJourneyVModel> ParseToVModel(IEnumerable<TransactionJourney> listModel)
        {
            var result = new List<TransactionJourneyVModel>();
            foreach (TransactionJourney model in listModel)
            {
                var vmodel = TransactionJourneyVModel.ToVModel(model);

                result.Add(vmodel);
            }
            return result;
        }
    }
}
