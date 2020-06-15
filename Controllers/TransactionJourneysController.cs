using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionJourneysController : ControllerBase
    {
        private readonly PLSystemContext _context;

        public TransactionJourneysController(PLSystemContext context)
        {
            _context = context;
        }

        // GET: api/TransactionJourneys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionJourney>>> GetTransactionJourney()
        {
            return await _context.TransactionJourney.ToListAsync();
        }

        // GET: api/TransactionJourneys/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionJourney>> GetTransactionJourney(long id)
        {
            var transactionJourney = await _context.TransactionJourney.FindAsync(id);

            if (transactionJourney == null)
            {
                return NotFound();
            }

            return transactionJourney;
        }

        // PUT: api/TransactionJourneys/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionJourney(long id, TransactionJourney transactionJourney)
        {
            if (id != transactionJourney.Id)
            {
                return BadRequest();
            }

            _context.Entry(transactionJourney).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionJourneyExists(id))
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

        // POST: api/TransactionJourneys
        [HttpPost]
        public async Task<ActionResult<TransactionJourney>> PostTransactionJourney(TransactionJourney transactionJourney)
        {
            _context.TransactionJourney.Add(transactionJourney);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransactionJourney", new { id = transactionJourney.Id }, transactionJourney);
        }

        // DELETE: api/TransactionJourneys/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TransactionJourney>> DeleteTransactionJourney(long id)
        {
            var transactionJourney = await _context.TransactionJourney.FindAsync(id);
            if (transactionJourney == null)
            {
                return NotFound();
            }

            _context.TransactionJourney.Remove(transactionJourney);
            await _context.SaveChangesAsync();

            return transactionJourney;
        }

        private bool TransactionJourneyExists(long id)
        {
            return _context.TransactionJourney.Any(e => e.Id == id);
        }
    }
}
