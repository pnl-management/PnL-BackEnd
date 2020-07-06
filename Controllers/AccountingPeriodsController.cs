using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PnLReporter.EnumInfo;
using PnLReporter.Models;
using PnLReporter.Service;
using PnLReporter.ViewModels;

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

        [HttpGet("/api/periods")]
        public ActionResult GetAccountingPeriod()
        {
            var user = this.GetCurrentUserInfo();
            
            return Ok(_service.GetListByBrand(user.Brand.Id));
        }

        [HttpGet("/api/periods/{id}")]
        public ActionResult GetAccountingPeriod(int id)
        {
            var accountingPeriod = _service.GetById(id);

            if (accountingPeriod == null)
            {
                return NotFound();
            }

            var user = this.GetCurrentUserInfo();

            if (user.Brand.Id != accountingPeriod.Brand.Id) return Forbid();

            return Ok(accountingPeriod);
        }

        [HttpPut("/api/periods/{id}")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public IActionResult PutAccountingPeriod(int id, AccountingPeriodVModel period)
        {
            if (period.Id != id) period.Id = id;

            var current = _service.GetById(id);

            if (current == null) return NotFound();

            var user = this.GetCurrentUserInfo();
            if (current.Brand.Id != user.Brand.Id) return Forbid();

            var result = _service.Update(period);
            if (result == null) return BadRequest();

            return Ok(result);
        }

        [HttpPost("/api/periods")]
        public ActionResult PostAccountingPeriod(AccountingPeriodVModel period)
        {
            var result = _service.Insert(period);

            return CreatedAtAction("GetAccountingPeriod", result);
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
