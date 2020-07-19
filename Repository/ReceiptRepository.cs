using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;

namespace PnLReporter.Repository
{
    public interface IReceiptRepository
    {

    }
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly PLSystemContext _context;

        public ReceiptRepository(PLSystemContext context)
        {
            this._context = context;
        }
    }
}
