using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PnLReporter.Repository
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> ListInvestorIndexTransactions(int participantsId);
        IEnumerable<Transaction> ListStoreTransactionInCurrentPeroid(int participantsId);
        IEnumerable<Transaction> ListWaitingForAccountantTransaction(int participantId);
        IEnumerable<Transaction> ListWaitingForStoreTransaction(int participants);
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
                .Include(trans => trans.CreatedByNavigation)
                .Include(trans => trans.Brand)
                .Include(trans => trans.Period)
                .Include(trans => trans.Category)
                .Include(trans => trans.Store)
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
                .Include(trans => trans.CreatedByNavigation)
                .Include(trans => trans.Brand)
                .Include(trans => trans.Period)
                .Include(trans => trans.Category)
                .Include(trans => trans.Store)
                .Where(record =>
                    record.PeriodId == currentPeriod.Id
                );
        }

        public IEnumerable<Transaction> ListWaitingForAccountantTransaction(int participantId)
        {
            return _context.Transaction
                .Include(trans => trans.CreatedByNavigation)
                .Include(trans => trans.Brand)
                .Include(trans => trans.Period)
                .Include(trans => trans.Category)
                .Include(trans => trans.Store)
                .Where(record =>
                    record.BrandId == _context.BrandParticipantsDetail
                        .Where(b => b.ParticipantsId == participantId)
                        .FirstOrDefault<BrandParticipantsDetail>()
                        .BrandId
                    //&&
                    //(new int?[] { 201, 202, 103 })
                    //.Contains(
                    //    record.TransactionJourney.FirstOrDefault().Status)
                    )
                .OrderByDescending(record => record.CreatedTime)
                .ToList();      
        }

        public IEnumerable<Transaction> ListWaitingForStoreTransaction(int participants)
        {
            return _context.Transaction
                .Include(trans => trans.CreatedByNavigation)
                .Include(trans => trans.Brand)
                .Include(trans => trans.Period)
                .Include(trans => trans.Category)
                .Include(trans => trans.Store)
                .Where(record =>
                    record.CreatedBy == participants
                    &&
                    (new int?[] { 102, 302 })
                    .Contains(
                        _context.TransactionJourney
                        .Where(j => j.TransactionId == record.Id)
                        .OrderByDescending(j => j.CreatedTime)
                        .FirstOrDefault()
                        .Status
                        )
                    )
                .OrderByDescending(record => record.CreatedTime)
                .ToList();
        }
    }
}
