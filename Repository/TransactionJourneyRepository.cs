using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;
using PnLReporter.ViewModels;

namespace PnLReporter.Repository
{
    public interface ITransactionJourneyRepository
    {
        TransactionJourney GetLastestStatus(long transactionId);
        TransactionJourney AddStatus(TransactionJourneyVModel journey);
    }
    public class TransactionJourneyRepository : ITransactionJourneyRepository
    {
        private PLSystemContext _context;

        public TransactionJourneyRepository(PLSystemContext context)
        {
            _context = context;
        }

        public TransactionJourney AddStatus(TransactionJourneyVModel journey)
        {
            TransactionJourney model = new TransactionJourney()
            {
                Status = journey.Status,
                CreatedBy = journey.CreatedByParticipant.Id,
                FeedBack = journey.FeedBack,
                CreatedTime = journey.CreatedTime,
                TransactionId = journey.Transaction.Id
            };

            _context.TransactionJourney.Add(model);
            _context.SaveChanges();
            return model;
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
