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
using PnLReporter.EnumInfo;
using PnLReporter.Models;
using PnLReporter.Service;
using PnLReporter.ViewModels;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TransactionJourneysController : ControllerBase
    {
        private readonly PLSystemContext _context;
        private readonly TransactionJourneyService _service;

        public TransactionJourneysController(PLSystemContext context)
        {
            _context = context;
            _service = new TransactionJourneyService(context);
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

        // POST: api/TransactionJourneys
        [HttpPost("/api/transactions/{transactionId}/journey")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public ActionResult PostTransactionJourney(long transactionId, String type, TransactionJourneyVModel transactionJourney)
        {
            var user = this.GetCurrentUserInfo();
            var transactionService = new TransactionService(_context);
            var currentTransaction = transactionService.GetById(transactionId);

            if (currentTransaction == null) return NotFound("Transaction ID not found");

            if (currentTransaction.Brand.Id != user.Brand.Id) return Forbid();

            transactionJourney.CreatedByParticipant = new ParticipantVModel() { Id = user.Id };

            TransactionJourneyVModel result;
            try
            {
                result = _service.JudgeTransaction(transactionJourney, type, user.Role + "");
            } 
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Created("", result);
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
