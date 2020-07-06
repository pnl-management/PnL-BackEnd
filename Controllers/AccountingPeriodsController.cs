using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;
using PnLReporter.Service;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountingPeriodsController : ControllerBase
    {
        private readonly PLSystemContext _context;
        private readonly IAccountingPeriodService _service;

        public AccountingPeriodsController(PLSystemContext context)
        {
            _context = context;
            _service = new AccountingPeriodService(context);
        }

        // GET: api/AccountingPeriods
        [HttpGet("/api/periods")]
        public ActionResult GetAccountingPeriod()
        {
            return Ok();
        }

        // GET: api/AccountingPeriods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountingPeriod>> GetAccountingPeriod(int id)
        {
            var accountingPeriod = await _context.AccountingPeriod.FindAsync(id);

            if (accountingPeriod == null)
            {
                return NotFound();
            }

            return accountingPeriod;
        }

        // PUT: api/AccountingPeriods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccountingPeriod(int id, AccountingPeriod accountingPeriod)
        {
            if (id != accountingPeriod.Id)
            {
                return BadRequest();
            }

            _context.Entry(accountingPeriod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountingPeriodExists(id))
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

        // POST: api/AccountingPeriods
        [HttpPost]
        public async Task<ActionResult<AccountingPeriod>> PostAccountingPeriod(AccountingPeriod accountingPeriod)
        {
            _context.AccountingPeriod.Add(accountingPeriod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccountingPeriod", new { id = accountingPeriod.Id }, accountingPeriod);
        }

        // DELETE: api/AccountingPeriods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AccountingPeriod>> DeleteAccountingPeriod(int id)
        {
            var accountingPeriod = await _context.AccountingPeriod.FindAsync(id);
            if (accountingPeriod == null)
            {
                return NotFound();
            }

            _context.AccountingPeriod.Remove(accountingPeriod);
            await _context.SaveChangesAsync();

            return accountingPeriod;
        }

        private bool AccountingPeriodExists(int id)
        {
            return _context.AccountingPeriod.Any(e => e.Id == id);
        }
    }
}
