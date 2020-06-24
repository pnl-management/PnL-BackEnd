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
        TransactionJourneyVModel AddStatus(TransactionJourneyVModel journey);
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

        public TransactionJourneyVModel AddStatus(TransactionJourneyVModel journey)
        {
            journey.Id = null;
            journey.CreatedTime = DateTime.Now;

            return ParseToVModel(new List<TransactionJourney> { _repository.AddStatus(journey) }).FirstOrDefault();
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

        private List<TransactionJourneyVModel> ParseToVModel(List<TransactionJourney> listModel)
        {
            var result = new List<TransactionJourneyVModel>();
            foreach (TransactionJourney model in listModel)
            {
                var vmodel = new TransactionJourneyVModel()
                {
                    Id = model.Id,
                    Status = model.Status,
                    FeedBack = model.FeedBack,
                    CreatedByParticipant = model.CreatedByNavigation != null ? new ParticipantVModel()
                    {
                        Id = model.CreatedByNavigation.Id,
                        Username = model.CreatedByNavigation.Username,
                        Fullname = model.CreatedByNavigation.Fullname,
                        CreatedTime = model.CreatedByNavigation.CreatedTime,
                        LastModified = model.CreatedByNavigation.LastModified
                    } : null,
                };

                result.Add(vmodel);
            }
            return result;
        }
    }
}
