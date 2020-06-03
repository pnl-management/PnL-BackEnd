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
        IEnumerable<Transaction> ListWaitingForAccountantTransaction(int participantId);
        IEnumerable<Transaction> ListWaitingForStoreTransaction(int participants);
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

        public IEnumerable<Transaction> ListWaitingForAccountantTransaction(int participantId)
        {
            return _repository.ListWaitingForAccountantTransaction(participantId);
        }

        public IEnumerable<Transaction> ListWaitingForStoreTransaction(int participants)
        {
            return _repository.ListWaitingForStoreTransaction(participants);
        }
    }
}
