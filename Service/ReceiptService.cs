using PnLReporter.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PnLReporter.Models;

namespace PnLReporter.Service
{
    public interface IReceiptService
    {

    }
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _repository;

        public ReceiptService(PLSystemContext context)
        {
            _repository = new ReceiptRepository(context);
        }
    }
}
