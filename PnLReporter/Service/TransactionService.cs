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
    }
    public class TransactionService : ITransactionService
    {
        private ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
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

        private IEnumerable<TransactionVModel> ParseToTransactionVModel(IEnumerable<Transaction> transList)
        {
            var transVModelLst = new List<TransactionVModel>();
            System.Diagnostics.Debug.WriteLine(transVModelLst.Count);

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
                            Name = trans.Category.Name
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
                        } : null
                    });
                });
            }

            return transVModelLst;
        }
    }
}
