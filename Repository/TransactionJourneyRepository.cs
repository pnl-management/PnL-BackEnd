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
                CreatedTime = DateTime.UtcNow.AddHours(7),
                TransactionId = journey.Transaction.Id
            };

            _context.TransactionJourney.Add(model);
            _context.SaveChanges();
            return model;
        }

        public TransactionJourney FindById(long id)
        {
            return _context.TransactionJourney
                .Include(record => record.CreatedByNavigation)
                .Include(record => record.Transaction)
                .Include(record => record.Transaction.Store)
                .Include(record => record.Transaction.Brand)
                .Include(record => record.Transaction.Period)
                .Include(record => record.Transaction.Category)
                .Include(record => record.Transaction.CreatedByNavigation)
                .AsNoTracking()
                .Where(record => record.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<TransactionJourney> GetJourneyOfTransaction(long transactionId)
        {
            return _context.TransactionJourney
                .Include(record => record.CreatedByNavigation)
                .Include(record => record.Transaction)
                .Include(record => record.Transaction.Store)
                .Include(record => record.Transaction.Brand)
                .Include(record => record.Transaction.Period)
                .Include(record => record.Transaction.Category)
                .Include(record => record.Transaction.CreatedByNavigation)
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
