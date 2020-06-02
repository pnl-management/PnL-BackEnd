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
        IEnumerable<Transaction> ListTransactions();
        IEnumerable<Transaction> ListInvestorIndexTransactions(string username);
    }
    public class TransactionService : ITransactionService
    {
        private ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Transaction> ListInvestorIndexTransactions(string username)
        {
            return _repository.ListInvestorIndexTransactions(username);
        }

        public IEnumerable<Transaction> ListTransactions()
        {
            return _repository.ListTransactions();
        }
    }
}
