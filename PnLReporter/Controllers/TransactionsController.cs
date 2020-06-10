﻿using System;
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

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
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
        [Authorize(Roles = "accountant,investor")]
        public ActionResult<IEnumerable<Object>> GetTransaction(string sort, string filter, string query, string offset, string limit)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string participantIdVal = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            long userId;

            long.TryParse(participantIdVal, out userId);
            IParticipantService participantService = new ParticipantService(_context);

            int brandId = participantService.FindByUserId(userId).Brand.Id;

            IEnumerable<Object> result;
            // paging implement
            int offsetVal = 0, limitVal = 0;
            int.TryParse(offset, out offsetVal);
            int.TryParse(limit, out limitVal);
            if (limitVal > 20) limitVal = 20;
            if (limitVal < 5) limitVal = 5;

            // query implement
            result = _service.QueryListByFieldAndBrand(query, offsetVal, limitVal, brandId);

            // sort implement
            if (!String.IsNullOrEmpty(sort))
            {
                result = _service.SortList(sort, (IEnumerable<TransactionVModel>)result);
            }

            // filter implement
            if (!String.IsNullOrEmpty(filter))
            {
                result = _service.FilterFieldOut(filter, (IEnumerable<TransactionVModel>)result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("Index")]
        public ActionResult<IEnumerable<TransactionVModel>> GetTransactionOnIndexPg()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string role = identity.FindFirst(ClaimTypes.Role).Value;
            string participantIdVal = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            
            int participantId;
            if (int.TryParse(participantIdVal, out participantId)) {
                switch (role)
                {
                    case "investor":
                        return Ok(_service.ListInvestorIndexTransactions(participantId));
                    case "store-manager":
                        return Ok(
                            new { currentPeriod = _service.ListStoreTransactionInCurrentPeroid(participantId),
                                waitingTransaction = _service.ListWaitingForStoreTransaction(participantId)}
                            );
                    case "accountant":
                        return Ok(_service.ListWaitingForAccountantTransaction(participantId));
                }
            }

            return BadRequest(new { role = role, id = participantId });
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(long id)
        {
            var transaction = await _context.Transaction.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // PUT: api/Transactions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(long id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return BadRequest();
            }

            _context.Entry(transaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            _context.Transaction.Add(transaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransaction", new { id = transaction.Id }, transaction);
        }

        // DELETE: api/Transactions/5
        [HttpDelete("{id}")]
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
        }
    }
}
