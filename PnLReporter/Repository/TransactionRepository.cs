using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PnLReporter.Repository
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> ListInvestorIndexTransactions(int participantsId);
        IEnumerable<Transaction> ListStoreTransactionInCurrentPeroid(int participantsId);
    }

    public class TransactionRepository : ITransactionRepository
    {
        private PLSystemContext _context;

        public TransactionRepository(PLSystemContext context)
        {
            _context = context;
        }

        public IEnumerable<Transaction> ListInvestorIndexTransactions(int participantsId)
        {
            var investorBrand = _context.BrandParticipantsDetail
                .Where(record => record.ParticipantsId == participantsId)
                .Select(record => record.BrandId)
                .FirstOrDefault();

            return _context.Transaction
                .Where(record =>
                    record.BrandId == investorBrand
                    &&
                    _context.TransactionJourney
                        .Where(jorney =>
                            jorney.TransactionId == record.Id)
                        .OrderByDescending(jorney => jorney.CreatedTime)
                        .FirstOrDefault().Status == 303
                ).Take(5).ToList();
        }

        public IEnumerable<Transaction> ListStoreTransactionInCurrentPeroid(int participantId)
        {
            var currentParticipant = _context.BrandParticipantsDetail
                .Where(record => record.ParticipantsId == participantId)
                .FirstOrDefault();

            var currentPeriod = _context.AccountingPeriod
                .Where(record =>
                    record.BrandId == currentParticipant.BrandId
                    &&
                    record.Status == 1
                ).FirstOrDefault<AccountingPeriod>();

            return _context.Transaction
                .Where(record =>
                    record.PeriodId == currentPeriod.Id
                );
        }
    }
}
