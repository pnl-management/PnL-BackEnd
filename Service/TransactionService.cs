using PnLReporter.Models;
using PnLReporter.ViewModels;
using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PnLReporter.Helper;
using PnLReporter.EnumInfo;

namespace PnLReporter.Service
{
    public interface ITransactionService
    {
        IEnumerable<TransactionVModel> ListInvestorIndexTransactions(int participantId);
        IEnumerable<TransactionVModel> ListStoreTransactionInCurrentPeroid(int participantsId);
        IEnumerable<TransactionVModel> ListWaitingForAccountantTransaction(int participantId);
        IEnumerable<TransactionVModel> ListWaitingForStoreTransaction(int participants);
        IEnumerable<TransactionVModel> QueryListByFieldAndBrand(string query, string sort, int offset, int limit, int brandId);
        IEnumerable<Object> LimitList(int offset, int limit, IEnumerable<TransactionVModel> list);
        IEnumerable<Object> FilterFieldOut(string filter, IEnumerable<TransactionVModel> list);
        int GetQueryListLength(string query, int? brandId);
        bool CheckTransactionBelongToBrand(long? tranId, int? brandId);
        bool CheckTransactionBelongToStore(long? tranId, int? storeId);
        TransactionVModel GetById(long tranId);
        TransactionVModel UpdateTransaction(TransactionVModel transaction);
        TransactionVModel CreateTransaction(TransactionVModel transaction);
        bool IsTransactionCanModified(long transactionId);
    }
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        private readonly PLSystemContext _context;

        private readonly ITransactionJourneyService _journeyService;

        public TransactionService(PLSystemContext context)
        {
            _context = context;
            _repository = new TransactionRepository(context);
            _journeyService = new TransactionJourneyService(context);
        }

        public bool CheckTransactionBelongToBrand(long? tranId, int? brandId)
        {
            return _repository.CheckTransactionBelongToBrand(tranId, brandId);
        }

        public bool CheckTransactionBelongToStore(long? tranId, int? storeId)
        {
            return _repository.CheckTransactionBelongToStore(tranId, storeId);
        }

        public TransactionVModel CreateTransaction(TransactionVModel transaction)
        {
            
            var model = new Transaction()
            {
                Name = transaction.Name,
                Description = transaction.Description,
                Value = transaction.Value,
                CategoryId = transaction.Category.Id,
                BrandId = transaction.Brand.Id,
                StoreId = transaction.Store.Id,
                CreatedBy = transaction.CreateByParticipant.Id,
                CreatedTime = DateTime.Now
            };
            var result = _repository.CreateTransaction(model);

            TransactionJourneyVModel journey = new TransactionJourneyVModel()
            {
                Status = TransactionStatusConst.STORE_CREATED,
                Transaction = new TransactionVModel() { Id = result.Id },
                CreatedByParticipant = new ParticipantVModel() { Id = transaction.CreateByParticipant.Id }
            };

            _journeyService.AddStatus(journey);

            

            return this.ParseToTransactionVModel(new List<Transaction>() { result }).FirstOrDefault();
        }

        public IEnumerable<object> FilterFieldOut(string filter, IEnumerable<TransactionVModel> list)
        {
            IEnumerable<object> result = list;
            if (filter == null)
            {
                return list;
            }
            filter = filter.ToLower();
            if (filter != "any")
            {
                List<string> fields = filter.Split(",").ToList();
                string filterStr = "";

                foreach (string field in fields)
                {
                    switch (field)
                    {
                        case "id":
                            filterStr += "Id,";
                            break;
                        case "name":
                            filterStr += "Name,";
                            break;
                        case "value":
                            filterStr += "Value,";
                            break;
                        case "description":
                            filterStr += "Description,";
                            break;
                        case "category":
                            filterStr += "Category,";
                            break;
                        case "period":
                            filterStr += "Period,";
                            break;
                        case "brand":
                            filterStr += "Brand,";
                            break;
                        case "store":
                            filterStr += "Store,";
                            break;
                        case "created-time":
                            filterStr += "CreatedTime,";
                            break;
                        case "created-by-participant":
                            filterStr += "CreateByParticipant,";
                            break;
                        case "lastest-status":
                            filterStr += "LastestStatus,";
                            break;
                    }
                }

                if (filterStr.Length > 0) filterStr = filterStr.Substring(0, filterStr.Length - 1);
                if (fields.Count > 0)
                {
                    result = list.Select(Helper.Helper.DynamicSelectGenerator<TransactionVModel>(filterStr));
                }
            }

            return result;
        }

        public TransactionVModel GetById(long tranId)
        {
            var transaction = _repository.GetById(tranId);

            if (transaction != null)
            {
                return this.ParseToTransactionVModel(new List<Transaction>{transaction}).FirstOrDefault();
            }
            return null;
        }

        public int GetQueryListLength(string query, int? brandId)
        {
            if (query == null) query = "";
            return _repository.GetQueryListLength(query, brandId);
        }

        public bool IsTransactionCanModified(long transactionId)
        {
            ITransactionJourneyService jorneyService = new TransactionJourneyService(_context);
            var lastestStatus = jorneyService.GetLastestStatus(transactionId);

            if (lastestStatus == null) throw new Exception(TransactionExceptionMessage.CUR_STATUS_NOT_FOUND);

            var modifiedStatus = new List<int>()
                {
                    TransactionStatusConst.ACC_REQ_MODIFIED,
                    TransactionStatusConst.INVESTOR_REQ_MODIFIED,
                    TransactionStatusConst.STORE_CREATED,
                    TransactionStatusConst.STORE_MODIFIED
                };

            if (modifiedStatus.Contains(lastestStatus.Status ?? default))
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Object> LimitList(int offset, int limit, IEnumerable<TransactionVModel> list)
        {
            return list.Skip(offset).Take(limit);
        }

        public IEnumerable<TransactionVModel> ListInvestorIndexTransactions(int participantId)
        {
            return this.ParseToTransactionVModel(_repository.ListInvestorIndexTransactions(participantId));
        }

        public IEnumerable<TransactionVModel> ListStoreTransactionInCurrentPeroid(int participantsId)
        {
            return this.ParseToTransactionVModel(_repository.ListStoreTransactionInCurrentPeroid(participantsId));
        }

        public IEnumerable<TransactionVModel> ListWaitingForAccountantTransaction(int participantId)
        {
            return this.ParseToTransactionVModel(_repository.ListWaitingForAccountantTransaction(participantId));
        }

        public IEnumerable<TransactionVModel> ListWaitingForStoreTransaction(int participants)
        {
            return this.ParseToTransactionVModel(_repository.ListWaitingForStoreTransaction(participants));
        }

        public IEnumerable<TransactionVModel> QueryListByFieldAndBrand(string query, string sort, int offset, int limit, int brandId)
        {
            if (String.IsNullOrEmpty(query))
            {
                query = "";
            }
            return this.ParseToTransactionVModel(_repository.QueryListByFieldAndBrand(query, sort, offset, limit, brandId));
        }

        public TransactionVModel UpdateTransaction(TransactionVModel transaction)
        {
            if (transaction != null)
            {
                if (this.IsTransactionCanModified(transaction.Id))
                {
                    var list = new List<Transaction>
                    {
                        _repository.UpdateTransaction(transaction)
                    };

                    TransactionJourneyVModel journey = new TransactionJourneyVModel()
                    {
                        Status = TransactionStatusConst.STORE_MODIFIED,
                        Transaction = new TransactionVModel() { Id = transaction.Id },
                        CreatedByParticipant = new ParticipantVModel() { Id = transaction.CreateByParticipant.Id }
                    };

                    _journeyService.AddStatus(journey);

                    return this.ParseToTransactionVModel(list).FirstOrDefault();
                }
                else
                {
                    throw new Exception(TransactionExceptionMessage.CUR_STATUS_CANNOT_MODIFIED);
                }
            }
            throw new Exception(TransactionExceptionMessage.OBJ_IS_NULL);
        }

        private IEnumerable<TransactionVModel> ParseToTransactionVModel(IEnumerable<Transaction> transList)
        {
            var transVModelLst = new List<TransactionVModel>();
            ITransactionJourneyService jorneyService = new TransactionJourneyService(_context);

            if (transList != null)
            {
                transList.ToList().ForEach(trans =>
                {
                    transVModelLst.Add(new TransactionVModel()
                    {
                        Id = trans.Id,
                        Name = trans.Name,
                        Value = trans.Value,
                        Description = trans.Description,
                        Category = trans.Category != null ? new TransactionCategoryVModel()
                        {
                            Id = trans.Category.Id,
                            Name = trans.Category.Name,
                            Type = trans.Category.Type
                        } : null,
                        Period = trans.Period != null ? new AccountingPeriodVModel()
                        {
                            Id = trans.Period.Id,
                            Title = trans.Period.Title,
                            Status = trans.Period.Status
                        } : null,
                        Brand = trans.Brand != null ? new BrandVModel()
                        {
                            Id = trans.Brand.Id,
                            Name = trans.Brand.Name
                        } : null,
                        Store = trans.Store != null ? new StoreVModel()
                        {
                            Id = trans.Store.Id,
                            Name = trans.Store.Name
                        } : null,
                        CreatedTime = trans.CreatedTime,
                        CreateByParticipant = trans.CreatedByNavigation != null ? new ParticipantVModel()
                        {
                            Id = trans.CreatedBy,
                            Username = trans.CreatedByNavigation.Username
                        } : null,
                        LastestStatus = jorneyService.GetLastestStatus(trans.Id)
                    });
                });
            }

            return transVModelLst;
        }
    }
}
