using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;

namespace PnLReporter.Repository
{
    public interface ITransactionJourneyRepository
    {
        TransactionJourney GetLastestStatus(long transactionId);
    }
    public class TransactionJourneyRepository : ITransactionJourneyRepository
    {
        private PLSystemContext _context;

        public TransactionJourneyRepository(PLSystemContext context)
        {
            _context = context;
        }

        public TransactionJourney GetLastestStatus(long transactionId)
        {
            return _context.TransactionJourney
                .Where(record => record.TransactionId == transactionId)
                .OrderByDescending(record => record.CreatedTime)
                .FirstOrDefault();
        }
    }
}
