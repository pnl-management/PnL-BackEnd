using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using PnLReporter.ViewModels;

namespace PnLReporter.Repository
{
    public interface ITransactionJourneyRepository
    {
        TransactionJourney GetLastestStatus(long transactionId);
        IEnumerable<TransactionJourney> GetJourneyOfTransaction(long transactionId);
        TransactionJourney AddStatus(TransactionJourneyVModel journey);
        TransactionJourney FindById(long id);
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

        public TransactionJourney FindById(long id)
        {
            return _context.TransactionJourney.AsNoTracking()
                .Include(record => record.Transaction)
                .Where(record => record.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<TransactionJourney> GetJourneyOfTransaction(long transactionId)
        {
            return _context.TransactionJourney
                .AsNoTracking()
                .Where(record => record.TransactionId == transactionId)
                .OrderByDescending(record => record.CreatedTime)
                .ToList();
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
