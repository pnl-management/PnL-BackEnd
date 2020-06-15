﻿using PnLReporter.Models;
using PnLReporter.ViewModels;
using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using PnLReporter.Helper;

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
    }
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        private readonly PLSystemContext _context;

        public TransactionService(PLSystemContext context)
        {
            _context = context;
            _repository = new TransactionRepository(context);
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

        public int GetQueryListLength(string query, int? brandId)
        {
            return _repository.GetQueryListLength(query, brandId);
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
                            Title = trans.Period.Title
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
