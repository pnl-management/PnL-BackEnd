using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.ViewModels
{
    public class TransactionJourneyVModel
    {
        public long? Id { get; set; }
        public int? Status { get; set; }
        public ParticipantVModel CreatedByParticipant { get; set; }
        public string FeedBack { get; set; }
        public DateTime? CreatedTime { get; set; }
        public TransactionVModel Transaction { get; set; }

        public static TransactionJourneyVModel ToVModel(TransactionJourney model)
        {
            if (model == null) return null;

            var vmodel = new TransactionJourneyVModel()
            {
                Id = model.Id,
                Status = model.Status,
                CreatedByParticipant = ParticipantVModel.ToVModel(model.CreatedByNavigation) ?? new ParticipantVModel() { Id = model.CreatedBy},
                FeedBack = model.FeedBack,
                CreatedTime = model.CreatedTime,
                Transaction = TransactionVModel.ToVModel(model.Transaction) ?? new TransactionVModel() { Id = model.TransactionId ?? -1}
            };

            return vmodel;
        }
    }
}
