using PnLReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PnLReporter.EnumInfo;
using PnLReporter.ViewModels;

namespace PnLReporter.Repository
{
    public interface ITransactionRepository
    {
        IEnumerable<Transaction> ListInvestorIndexTransactions(int participantsId);
        IEnumerable<Transaction> ListStoreTransactionInCurrentPeroid(int participantsId);
        IEnumerable<Transaction> ListWaitingForAccountantTransaction(int participantId);
        IEnumerable<Transaction> ListWaitingForStoreTransaction(int participants);
        IEnumerable<Transaction> QueryListByFieldAndBrand(string query, string sort, int offset, int limit, int? brandId);
        IEnumerable<Transaction> GetAllByBrand(int offset, int limit, int? brandId);
        int GetQueryListLength(string query, int? brandId);
        bool CheckTransactionBelongToBrand(long? tranId, int? brandId);
        bool CheckTransactionBelongToStore(long? tranId, int? storeId);
        Transaction GetById(long tranId);
        Transaction UpdateTransaction(TransactionVModel transaction);
        Transaction CreateTransaction(Transaction transaction);
        Transaction PutTransactionToPeriod(long tranId, int periodId);
    }

    public class TransactionRepository : ITransactionRepository
    {
        private PLSystemContext _context;

        public TransactionRepository(PLSystemContext context)
        {
            _context = context;
        }

        public IEnumerable<Transaction> GetAllByBrand(int offset, int limit, int? brandId)
        {
            return _context.Transaction
                .Include(trans => trans.CreatedByNavigation)
                .Include(trans => trans.Brand)
                .Include(trans => trans.Period)
                .Include(trans => trans.Category)
                .Include(trans => trans.Store)
                .Where(trans => trans.BrandId == ((brandId != null) ? brandId : trans.BrandId))
                .Skip(offset).Take(limit)
                .ToList();
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
                ).ToList();
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
                    &&
                    (new int?[] { 201, 202, 103 })
                    .Contains(
                        record.TransactionJourney.FirstOrDefault().Status)
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

        public IEnumerable<Transaction> QueryListByFieldAndBrand(string query, string sort, int offset, int limit, int? brandId)
        {
            return this.GenQueryStatement(query, sort, brandId).Skip(offset).Take(limit).ToList();
        }

        public int GetQueryListLength(string query, int? brandId)
        {
            return this.GenQueryStatement(query, "", brandId).Count();
        }

        private IQueryable<Transaction> GenQueryStatement(string query, string sort, int? brandId)
        {
            IQueryable<Transaction> result =
                _context.Transaction
                .Include(record => record.Category)
                .Include(record => record.Brand)
                .Include(record => record.Period)
                .Include(record => record.Store)
                .Include(record => record.CreatedByNavigation)
                .Where(trans => trans.BrandId == ((brandId != null) ? brandId : trans.BrandId));

            if (sort != null && sort.Length > 0)
            {
                switch (sort.ToLower().Trim())
                {
                    case "id-asc":
                        result = result.OrderBy(record => record.Id);
                        break;
                    case "id-des":
                        result = result.OrderByDescending(record => record.Id);
                        break;
                    case "value-asc":
                        result = result.OrderBy(record => long.Parse(record.Value));
                        break;
                    case "value-des":
                        result = result.OrderByDescending(record => long.Parse(record.Value));
                        break;
                    case "created-time-asc":
                        result = result.OrderBy(record => record.CreatedTime);
                        break;
                    case "created-time-des":
                        result = result.OrderByDescending(record => record.CreatedTime);
                        break;
                }
            }

            List<string> queryComponent = new List<string>();
            queryComponent = query.Split(",").ToList();

            string field;
            string opt;
            string value;

            List<int> lastestStatusList = new List<int>();

            foreach (string queryContent in queryComponent)
            {
                string queryContentVal = queryContent.Trim();
                if (queryContentVal.Length > 0)
                {
                    field = queryContentVal.Substring(0, queryContentVal.IndexOf("["));
                    if (String.IsNullOrEmpty(field))
                    {
                        break;
                    }

                    if (queryContentVal.IndexOf("[") == -1 || queryContentVal.IndexOf("]") == -1 ||
                        queryContentVal.IndexOf("[") > queryContentVal.IndexOf("]")) break;

                    opt = queryContentVal.Substring(queryContentVal.IndexOf("[") + 1,
                        queryContentVal.IndexOf("]") - queryContentVal.IndexOf("[") - 1);

                    value = queryContentVal
                        .Substring(queryContentVal.IndexOf("]") + 1,
                        queryContentVal.Length - (queryContentVal.IndexOf("]") + 1));

                    switch (field)
                    {
                        case "name":
                            switch (opt)
                            {
                                case "eq":
                                    result = result.Where(record => record.Name == value);
                                    break;
                                case "like":
                                    result = result.Where(record => record.Name.Contains(value));
                                    break;
                            }
                            break;
                        case "value":
                            long valueVal;
                            if (long.TryParse(value, out valueVal))
                            {
                                switch (opt)
                                {
                                    case "lt":
                                        result = result.Where(record => long.Parse(record.Value) < valueVal);
                                        break;
                                    case "gt":
                                        result = result.Where(record => long.Parse(record.Value) > valueVal);
                                        break;
                                    case "eq":
                                        result = result.Where(record => long.Parse(record.Value) == valueVal);
                                        break;
                                    case "lte":
                                        result = result.Where(record => long.Parse(record.Value) <= valueVal);
                                        break;
                                    case "gte":
                                        result = result.Where(record => long.Parse(record.Value) >= valueVal);
                                        break;
                                }
                            }
                            break;
                        case "store-id":
                            switch (opt)
                            {
                                case "eq":
                                    int storeId;
                                    if (!int.TryParse(value, out storeId))
                                    {
                                        storeId = -1;
                                    }
                                    result = result.Where(record => record.StoreId == storeId);
                                    break;
                            }
                            break;
                        case "lastest-status":
                            int status;
                            if (int.TryParse(value, out status))
                            {
                                if (opt == "eq")
                                {
                                    lastestStatusList.Add(status);
                                }
                            }
                            break;
                    }
                }
            }

            if (lastestStatusList.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Length " + lastestStatusList.Count);
                result = result.Where(record =>
                    lastestStatusList.Contains(_context.TransactionJourney
                        .Where(j => j.TransactionId == record.Id)
                        .OrderByDescending(j => j.CreatedTime)
                        .FirstOrDefault()
                        .Status ?? default(int))
                );
            }

            return result;
        }

        public bool CheckTransactionBelongToBrand(long? tranId, int? brandId)
        {
            bool check = false;
            var currentTran = _context.Transaction.Where(record => record.Id == tranId).FirstOrDefault();
            if (currentTran != null)
            {
                if (currentTran.BrandId == brandId) check = true;
            }
            return check;
        }

        public bool CheckTransactionBelongToStore(long? tranId, int? storeId)
        {
            bool check = false;
            var currentTran = _context.Transaction.Where(record => record.Id == tranId).FirstOrDefault();
            if (currentTran != null)
            {
                if (currentTran.StoreId == storeId) check = true;
            }
            return check;
        }

        public Transaction GetById(long tranId)
        {
            var result = _context.Transaction
                .AsNoTracking()
                .Include(record => record.Period)
                .Include(record => record.Brand)
                .Include(record => record.Store)
                .Include(record => record.Category)
                .Include(record => record.CreatedByNavigation)
                .Where(record => record.Id == tranId)
                .FirstOrDefault();

            result.Period = _context.AccountingPeriod.Find(result.PeriodId);
            result.Brand = _context.Brand.Find(result.BrandId);
            result.Store = _context.Store.Find(result.StoreId);
            result.Category = _context.TransactionCategory.Find(result.CategoryId);
            result.CreatedByNavigation = _context.Participant.Find(result.CreatedBy);

            return result;
        }

        public Transaction UpdateTransaction(TransactionVModel transaction)
        {
            var curTransaction = _context.Transaction.Where(record => record.Id == transaction.Id).FirstOrDefault();

            if (curTransaction == null) throw new Exception(TransactionExceptionMessage.TRANSACTION_NOT_FOUND);

            curTransaction.Name = transaction.Name;
            curTransaction.Value = transaction.Value;
            curTransaction.Description = transaction.Description;
            curTransaction.CategoryId = transaction.Category.Id;

            _context.Entry(curTransaction).State = EntityState.Modified;
            _context.SaveChanges();

            return curTransaction;
        }

        public Transaction CreateTransaction(Transaction transaction)
        {
            _context.Transaction.Add(transaction);
            _context.SaveChanges();
            return transaction;
        }

        public Transaction PutTransactionToPeriod(long tranId, int periodId)
        {
            var current = this.GetById(tranId);
            current.PeriodId = periodId;

            _context.Entry(current).State = EntityState.Modified;
            _context.SaveChanges();

            return current;
        }
    }
}
