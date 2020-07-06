using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using PnLReporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Repository
{
    public interface IEvidenceRepository
    {
        IEnumerable<Evidence> GetListEvidenceOfTransaction(long transactionId);
        Evidence GetById(long evidenceId);
        Evidence UpdateEvidence(EvidenceVModel evidence);
        IEnumerable<Evidence> InsertEvidences(IEnumerable<Evidence> evidencesLst);
        Evidence DeleteEvidence(long id);
    }
    public class EvidenceRepository : IEvidenceRepository
    {
        private readonly PLSystemContext _context;

        public EvidenceRepository(PLSystemContext context)
        {
            _context = context;
        }

        public Evidence DeleteEvidence(long id)
        {
            var current = this.GetById(id);
            if (current == null) return null;
            _context.Evidence.Remove(current);
            _context.SaveChanges();
            return current;
        }

        public Evidence GetById(long evidenceId)
        {
            return _context.Evidence
                .Include(record => record.Transaction)
                .Include(record => record.Transaction.Store)
                .Include(record => record.Transaction.Brand)
                .Include(record => record.Transaction.CreatedByNavigation)
                .Where(record => record.Id == evidenceId)
                .FirstOrDefault();
        }

        public IEnumerable<Evidence> GetListEvidenceOfTransaction(long transactionId)
        {
            return _context.Evidence.Where(record => record.TransactionId == transactionId).ToList();
        }

        public IEnumerable<Evidence> InsertEvidences(IEnumerable<Evidence> evidencesLst)
        {
            foreach (var evidence in evidencesLst)
            {
                _context.Evidence.Add(evidence);
            }
            _context.SaveChanges();

            return evidencesLst;
        }

        public Evidence UpdateEvidence(EvidenceVModel evidence)
        {
            var model = this.GetById(evidence.Id ?? default);

            if (model == null) return null;

            model.Title = evidence.Title;
            model.Description = evidence.Description;

            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();

            return model;
        }
    }
}
