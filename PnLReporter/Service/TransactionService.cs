using PnLReporter.Models;
using PnLReporter.ViewModels;
using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Service
{
    public interface ITransactionService
    {
        IEnumerable<TransactionVModel> ListInvestorIndexTransactions(int participantId);
        IEnumerable<TransactionVModel> ListStoreTransactionInCurrentPeroid(int participantsId);
        IEnumerable<TransactionVModel> ListWaitingForAccountantTransaction(int participantId);
        IEnumerable<TransactionVModel> ListWaitingForStoreTransaction(int participants);
        IEnumerable<TransactionVModel> SortList(string sortOrder);
        IEnumerable<TransactionVModel> QueryListByField(string query);
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

        public IEnumerable<TransactionVModel> QueryListByField(string query)
        {
            return this.ParseToTransactionVModel(_repository.QueryListByField(query));
        }

        public IEnumerable<TransactionVModel> SortList(string sortOrder, IEnumerable<TransactionVModel> list)
        {
            switch (sortOrder)
            {
                case "id-asc":
                    list.OrderBy(record => record.Id);
                    break;
                case "id-des":
                    list.OrderByDescending(record => record.Id);
                    break;
                case "value-asc":
                    list.OrderBy(record =>
                    {
                        long result;
                        long.TryParse(record.Value, out result);
                        return result;
                    });
                    break;
                case "value-des":
                    list.OrderByDescending(record =>
                    {
                        long result;
                        long.TryParse(record.Value, out result);
                        return result;
                    });
                    break;
                case "created-time-asc":
                    list.OrderBy(record => record.CreatedTime);
                    break;
                case "created-time-des":
                    list.OrderByDescending(record => record.CreatedTime);
                    break;
            }
            return list;
        }

        public IEnumerable<TransactionVModel> SortList(string sortOrder)
        {
            throw new NotImplementedException();
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
