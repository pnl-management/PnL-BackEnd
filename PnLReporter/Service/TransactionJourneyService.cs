using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.ViewModels;
using PnLReporter.Repository;
using PnLReporter.Models;

namespace PnLReporter.Service
{
    public interface ITransactionJourneyService
    {
        TransactionJourneyVModel GetLastestStatus(long transactionId);
    }
    public class TransactionJourneyService : ITransactionJourneyService
    {
        private readonly ITransactionJourneyRepository _repository;
        private readonly PLSystemContext _context;

        public TransactionJourneyService(PLSystemContext context)
        {
            _context = context;
            _repository = new TransactionJourneyRepository(context);
        }

        public TransactionJourneyVModel GetLastestStatus(long transactionId)
        {
            return _repository.GetLastestStatus(transactionId) != null ? new TransactionJourneyVModel()
            {
                Id = _repository.GetLastestStatus(transactionId).Id,
                Status = _repository.GetLastestStatus(transactionId).Status,
                FeedBack = _repository.GetLastestStatus(transactionId).FeedBack,
                CreatedTime = _repository.GetLastestStatus(transactionId).CreatedTime
            } : null;
        }
    }
}
