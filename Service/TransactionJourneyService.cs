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
            journey.CreatedTime = DateTime.Now;

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

            var accountantCanJudgeStt = new List<int>()
            {
                TransactionStatusConst.STORE_CREATED,
                TransactionStatusConst.STORE_MODIFIED
            };

            var investorCanJudgeStt = new List<int>()
            {
                TransactionStatusConst.ACC_APPROVED
            };

            switch (role)
            {
                case ParticipantsRoleConst.ACCOUNTANT:
                    if (accountantCanJudgeStt.Contains(lastestStatus.Status ?? default))
                    {
                        switch (type)
                        {
                            case TransactionJourneyReqType.APPROVE:
                                journey.Status = TransactionStatusConst.ACC_APPROVED;
                                break;
                            case TransactionJourneyReqType.REJECT:
                                journey.Status = TransactionStatusConst.ACC_CANCELED;
                                break;
                            case TransactionJourneyReqType.REQ_MODIFIED:
                                journey.Status = TransactionStatusConst.ACC_REQ_MODIFIED;
                                break;
                            case TransactionJourneyReqType.CANCELED_AFTER_CLOSE:
                                var transactionService = new TransactionService(_context);
                                var currentTran = transactionService.GetById(journey.Transaction.Id);

                                var listPeriodCanModifiedStatus = new List<int>()
                                {
                                    PeriodStatusConst.OPENING,
                                    PeriodStatusConst.RE_OPEN
                                };

                                if (listPeriodCanModifiedStatus.Contains(currentTran.Period.Status ?? default))
                                {
                                    journey.Status = TransactionStatusConst.CANCELED_AFTER_CLOSED;
                                }
                                else
                                {
                                    throw new Exception(TransactionExceptionMessage.CUR_STATUS_NOT_BE_JUDGE);
                                }
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
                var vmodel = new TransactionJourneyVModel()
                {
                    Id = model.Id,
                    Status = model.Status,
                    FeedBack = model.FeedBack,
                    CreatedByParticipant = model.CreatedByNavigation != null ? new ParticipantVModel()
                    {
                        Id = model.CreatedByNavigation.Id,
                        Username = model.CreatedByNavigation.Username,
                        Fullname = model.CreatedByNavigation.Fullname,
                        CreatedTime = model.CreatedByNavigation.CreatedTime,
                        LastModified = model.CreatedByNavigation.LastModified
                    } : null,
                    Transaction = model.Transaction != null ? new TransactionVModel()
                    {
                        Id = model.Transaction.Id,
                        Name = model.Transaction.Name,
                        Value = model.Transaction.Value,
                        Description = model.Transaction.Description,
                        Category = model.Transaction.Category != null ? new TransactionCategoryVModel()
                        {
                            Id = model.Transaction.Category.Id,
                            Name = model.Transaction.Category.Name,
                            Type = model.Transaction.Category.Type
                        } : null,
                        Period = model.Transaction.Period != null ? new AccountingPeriodVModel()
                        {
                            Id = model.Transaction.Period.Id,
                            Title = model.Transaction.Period.Title,
                            Status = model.Transaction.Period.Status
                        } : null,
                        Brand = model.Transaction.Brand != null ? new BrandVModel()
                        {
                            Id = model.Transaction.Brand.Id,
                            Name = model.Transaction.Brand.Name
                        } : null,
                        Store = model.Transaction.Store != null ? new StoreVModel()
                        {
                            Id = model.Transaction.Store.Id,
                            Name = model.Transaction.Store.Name
                        } : null,
                        CreatedTime = model.Transaction.CreatedTime,
                        CreateByParticipant = model.Transaction.CreatedByNavigation != null ? new ParticipantVModel()
                        {
                            Id = model.Transaction.CreatedBy,
                            Username = model.Transaction.CreatedByNavigation.Username
                        } : null,
                    } : null
                };

                result.Add(vmodel);
            }
            return result;
        }
    }
}
