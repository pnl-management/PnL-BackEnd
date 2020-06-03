using PnLReporter.Models;
using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Service
{
    public interface ITransactionService
    {
        IEnumerable<Transaction> ListInvestorIndexTransactions(int participantId);
        IEnumerable<Transaction> ListStoreTransactionInCurrentPeroid(int participantsId);
    }
    public class TransactionService : ITransactionService
    {
        private ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Transaction> ListInvestorIndexTransactions(int participantId)
        {
            return _repository.ListInvestorIndexTransactions(participantId);
        }

        public IEnumerable<Transaction> ListStoreTransactionInCurrentPeroid(int participantsId)
        {
            return _repository.ListStoreTransactionInCurrentPeroid(participantsId);
        }
    }
}
