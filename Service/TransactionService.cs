﻿using PnLReporter.Models;
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
        TransactionVModel PutTransactionToPeriod(long tranId, int periodId, int userId);
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
                CreatedBy = transaction.CreateBy.Id,
                CreatedTime = DateTime.UtcNow.AddHours(7)
            };
            var result = _repository.CreateTransaction(model);

            TransactionJourneyVModel journey = new TransactionJourneyVModel()
            {
                Status = TransactionStatusConst.STORE_CREATED,
                Transaction = new TransactionVModel() { Id = result.Id },
                CreatedByParticipant = new ParticipantVModel() { Id = transaction.CreateBy.Id }
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

        public TransactionVModel PutTransactionToPeriod(long tranId, int periodId, int userId)
        {
            var journeyService = new TransactionJourneyService(_context);
            var lastestStt = journeyService.GetLastestStatus(tranId);
            if (lastestStt.Status == TransactionStatusConst.INVESTOR_APPROVED)
            {
                var periodService = new AccountingPeriodService(_context);
                var period = periodService.GetById(periodId);

                var listOpenStt = new List<int>()
                {
                    PeriodStatusConst.OPENING,
                    PeriodStatusConst.RE_OPEN
                };

                if (listOpenStt.Contains(period.Status ?? -100))
                {
                    var model = _repository.PutTransactionToPeriod(tranId, periodId);

                    TransactionJourneyVModel journey = new TransactionJourneyVModel()
                    {
                        Status = TransactionStatusConst.DONE,
                        Transaction = new TransactionVModel() { Id = tranId },
                        CreatedByParticipant = new ParticipantVModel() { Id = userId }
                    };

                    _journeyService.AddStatus(journey);

                    return this.ParseToTransactionVModel(new List<Transaction>() { model }).FirstOrDefault();
                }
                else
                {
                    throw new Exception(TransactionExceptionMessage.PERIOD_NOT_OPEN);
                }
            }
            else
            {
                throw new Exception(TransactionExceptionMessage.CURRENT_STATUS_CANNOT_TO_PERIOD);
            }
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
                        CreatedByParticipant = new ParticipantVModel() { Id = transaction.CreateBy.Id }
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
            System.Diagnostics.Debug.WriteLine(transList.ToList().Count());

            if (transList != null)
            {
                transList.ToList().ForEach(trans =>
                {
                    var vmodel = TransactionVModel.ToVModel(trans);
                    vmodel.LastestStatus = jorneyService.GetLastestStatus(trans.Id);
                    transVModelLst.Add(vmodel);
                });
            }

            return transVModelLst;
        }
    }
}
