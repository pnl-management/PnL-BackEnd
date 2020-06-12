using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using PnLReporter.Service;
using PnLReporter.ViewModels;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransactionCategoriesController : ControllerBase
    {
        private readonly PLSystemContext _context;
        private readonly ITransactionCategoryService _service;

        public TransactionCategoriesController(PLSystemContext context)
        {
            _context = context;
            _service = new TransactionCategoryService(context);
        }

        // GET: api/TransactionCategories
        [HttpGet]
        [Route("/api/brands/transaction-categories")]
        public ActionResult<IEnumerable<TransactionCategory>> GetTransactionCategory(string sort, string filter, string query, int offset, int limit)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string participantIdVal = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            long userId;

            long.TryParse(participantIdVal, out userId);

            IParticipantService participantService = new ParticipantService(_context);

            int brandId = participantService.FindByUserId(userId).Brand.Id;

            IEnumerable<Object> result = null;

            // paging implement
            if (limit > 20) limit = 20;
            if (limit < 5) limit = 5;

            result = _service.QueryByBrand(query, brandId, offset, limit);

            result = _service.SortList(sort, (IEnumerable<TransactionCategoryVModel>) result);

            result = _service.FilterColumns(filter, (IEnumerable<TransactionCategoryVModel>)result);

            return Ok(result);
        }

        // GET: api/TransactionCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionCategory>> GetTransactionCategory(long id)
        {
            var transactionCategory = await _context.TransactionCategory.FindAsync(id);

            if (transactionCategory == null)
            {
                return NotFound();
            }

            return transactionCategory;
        }

        // PUT: api/TransactionCategories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionCategory(long id, TransactionCategory transactionCategory)
        {
            if (id != transactionCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(transactionCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionCategoryExists(id))
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

        // POST: api/TransactionCategories
        [HttpPost]
        public async Task<ActionResult<TransactionCategory>> PostTransactionCategory(TransactionCategory transactionCategory)
        {
            _context.TransactionCategory.Add(transactionCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransactionCategory", new { id = transactionCategory.Id }, transactionCategory);
        }

        // DELETE: api/TransactionCategories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TransactionCategory>> DeleteTransactionCategory(long id)
        {
            var transactionCategory = await _context.TransactionCategory.FindAsync(id);
            if (transactionCategory == null)
            {
                return NotFound();
            }

            _context.TransactionCategory.Remove(transactionCategory);
            await _context.SaveChangesAsync();

            return transactionCategory;
        }

        private bool TransactionCategoryExists(long id)
        {
            return _context.TransactionCategory.Any(e => e.Id == id);
        }
    }
}
