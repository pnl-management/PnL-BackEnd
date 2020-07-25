using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using PnLReporter.ViewModels;
using PnLReporter.Service;
using PnLReporter.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PnLReporter.Helper;
using PnLReporter.EnumInfo;

namespace PnLReporter.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransactionsController : ControllerBase
    {
        private readonly PLSystemContext _context;
        private ITransactionService _service;
        

        public TransactionsController(PLSystemContext context)
        {
            _context = context;
            _service = new TransactionService(_context);
        }

        // GET: api/Brand/Transactions
        [HttpGet]
        [Route("/api/brands/transactions")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public ActionResult<IEnumerable<Object>> GetTransactionByBrand(string sort, string filter, string query, string offset, string limit)
        {
            var user = this.GetCurrentUserInfo();

            int brandId = user.Brand.Id ?? 0;

            IEnumerable<Object> result;
            // paging implement
            int offsetVal = 0, limitVal = 0;
            int.TryParse(offset, out offsetVal);
            int.TryParse(limit, out limitVal);
            if (limitVal > 20) limitVal = 20;
            if (limitVal < 5) limitVal = 5;

            // query implement
            result = _service.QueryListByFieldAndBrand(query, sort, offsetVal, limitVal, brandId);

            // filter implement
            if (!String.IsNullOrEmpty(filter))
            {
                result = _service.FilterFieldOut(filter, (IEnumerable<TransactionVModel>)result);
            }

            return Ok(result);
        }

        // GET: api/Brand/Transactions/Length
        [HttpGet]
        [Route("/api/brands/transactions/length")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public ActionResult<IEnumerable<Object>> CountTransactionByBrand(string query)
        {
            var user = this.GetCurrentUserInfo();

            int brandId = user.Brand.Id ?? 0;

            var result = _service.GetQueryListLength(query, brandId);

            return Ok(result);
        }

        [HttpGet]
        [Route("index")]
        public ActionResult<IEnumerable<TransactionVModel>> GetTransactionOnIndexPg()
        {
            var user = this.GetCurrentUserInfo();

            switch (user.Role + "")
            {
                case ParticipantsRoleConst.INVESTOR:
                    return Ok(_service.ListInvestorIndexTransactions(user.Id));
                case ParticipantsRoleConst.STORE_MANAGER:
                    return Ok(
                        new
                        {
                            currentPeriod = _service.ListStoreTransactionInCurrentPeroid(user.Id),
                            waitingTransaction = _service.ListWaitingForStoreTransaction(user.Id)
                        }
                        );
                case ParticipantsRoleConst.ACCOUNTANT:
                    return Ok(_service.ListWaitingForAccountantTransaction(user.Id));
            }

            return BadRequest(new { role = user.Role, id = user.Id });
        }

        [HttpGet("/api/transactions/{id}")]
        public ActionResult<Transaction> GetTransaction(long id)
        {
            var user = this.GetCurrentUserInfo();

            IParticipantService participantService = new ParticipantService(_context);
            int brandId = user.Brand.Id ?? 0;

            switch (user.Role + "")
            {
                case ParticipantsRoleConst.INVESTOR:
                    if (!_service.CheckTransactionBelongToBrand(id, brandId))
                    {
                        return Forbid();
                    }
                    break;
                case ParticipantsRoleConst.ACCOUNTANT:
                    if (!_service.CheckTransactionBelongToBrand(id, brandId))
                    {
                        return Forbid();
                    }
                    break;
                case ParticipantsRoleConst.STORE_MANAGER:
                    int storeId = user.Store.Id ?? 0;
                    if (!_service.CheckTransactionBelongToStore(id, storeId))
                    {
                        return Forbid();
                    }
                    break;
            }

            return Ok(_service.GetById(id));
        }

        // PUT: api/stores/transactions/5
        [HttpPut("/api/transactions/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.ACCOUNTANT)]
        public IActionResult PutTransaction(long id, TransactionVModel transaction)
        {
            if (id != transaction.Id)
            {
                transaction.Id = id;
            }

            var result = "";

            var user = this.GetCurrentUserInfo();
            int brandId = user.Brand.Id ?? 0;
            transaction.CreateBy = new ParticipantVModel() { Id = user.Id };

            if (!_service.CheckTransactionBelongToBrand(transaction.Id, brandId)) return Forbid();

            try
            {
                return Ok(_service.UpdateTransaction(transaction));
            } 
            catch (Exception e)
            {
                result = e.Message;
            }

            return BadRequest(new {message = result});
        }

        [HttpPut("/api/transactions/{tranId}/periods/{periodId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.ACCOUNTANT)]
        public ActionResult PutTransactionToPeriod(long tranId, int periodId)
        {
            var user = this.GetCurrentUserInfo();
            var currentTran = _service.GetById(tranId);

            if (currentTran == null) return NotFound();

            if (user.Brand.Id != currentTran.Brand.Id) return Forbid();
            try
            {
                var result = _service.PutTransactionToPeriod(tranId, periodId, user.Id);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // POST: api/stores/transactions
        [HttpPost("/api/transactions")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.ACCOUNTANT)]
        public ActionResult PostTransaction(TransactionVModel transaction)
        {
            if (transaction.Category == null || transaction.Category.Id < 0)
            {
                return BadRequest(TransactionExceptionMessage.TRANSACTION_CATEGORY_IS_NULL);
            }

            if (transaction.Store == null || transaction.Store.Id == null || transaction.Store.Id < 0 )
            {
                return BadRequest(TransactionExceptionMessage.STORE_ID_IS_EMPTY);
            }

            var user = this.GetCurrentUserInfo();
            transaction.CreateBy = new ParticipantVModel() { Id = user.Id };
            transaction.Brand = new BrandVModel() { Id = user.Brand.Id };

            var result = _service.CreateTransaction(transaction, transaction.ListReceipt);

            return Created("", result);
        }

        // DELETE: api/Transactions/5
        /*[HttpDelete("{id}")]
        public async Task<ActionResult<Transaction>> DeleteTransaction(long id)
        {
            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transaction.Remove(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        private bool TransactionExists(long id)
        {
            return _context.Transaction.Any(e => e.Id == id);
        }*/

        private UserModel GetCurrentUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string role = identity.FindFirst(ClaimTypes.Role).Value;
            string participantIdVal = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            long participantsId;

            long.TryParse(participantIdVal, out participantsId);
            IParticipantService participantService = new ParticipantService(_context);

            return participantService.FindByUserId(participantsId);
        }
    }
}
