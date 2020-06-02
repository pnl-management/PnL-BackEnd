using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Repository
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> ListTransactions();
        IEnumerable<Transaction> ListInvestorIndexTransactions(string username);
    }

    public class TransactionRepository : ITransactionRepository
    {
        private PLSystemContext _context;

        public TransactionRepository(PLSystemContext context)
        {
            _context = context;
        }

        public IEnumerable<Transaction> ListInvestorIndexTransactions(string username)
        {
            var investor = _context.Participant
                .Where(record => record.Username == username)
                .FirstOrDefault<Participant>();

            return _context.Transaction
                .Where(record =>
                    record.CreatedByNavigation.BrandId == investor.BrandId
                    && 
                    _context.TransactionJorney
                        .Where(jorney => 
                            jorney.TransactionId == record.TransactionId)
                        .OrderByDescending(jorney => jorney.CreatedTime)
                        .FirstOrDefault().Status == 303
                        
                ).Take(5).ToList();
        }

        public IEnumerable<Transaction> ListTransactions()
        {
            return _context.Transaction.ToList();
        }
    }
}
