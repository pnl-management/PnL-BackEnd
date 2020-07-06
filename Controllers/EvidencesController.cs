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
    public class EvidencesController : ControllerBase
    {
        private readonly PLSystemContext _context;
        private readonly IEvidenceService _service;

        public EvidencesController(PLSystemContext context)
        {
            _context = context;
            _service = new EvidenceService(context);
        }

        //Get all evidences of a transactions
        [HttpGet("/api/transactions/{transactionId}/evidences")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetEvidenceLst(long transactionId)
        {
            var user = this.GetCurrentUserInfo();
            bool isValid = false;
            var transaction = (new TransactionService(_context)).GetById(transactionId);

            if (transaction == null) return NotFound();

            switch (user.Role?? default)
            {
                case ParticipantsRoleConst.ACCOUNTANT_ID:
                case ParticipantsRoleConst.INVESTOR_ID:
                    if (transaction.Brand.Id != user.Brand.Id)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                    
                    break;
                case ParticipantsRoleConst.STORE_MANAGER_ID:
                    if (transaction.Store.Id != transactionId)
                    {
                        isValid = false;
                    } else
                    {
                        isValid = true;
                    }
                    break;
            }

            if (isValid)
            {
                return Ok(_service.GetListEvidenceOfTransaction(transactionId));
            }

            return Forbid();
        }

        // GET: api/Evidences/5
        [HttpGet("/api/evidences/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetEvidence(long id)
        {
            var result = _service.GetById(id);
            if (result == null) return NotFound();

            var user = this.GetCurrentUserInfo();

            if (user.Brand.Id != result.Transaction.Brand.Id) return Forbid();

            return Ok(result);
        }

        // PUT: api/Evidences/5
        [HttpPut("/api/evidences/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public IActionResult PutEvidence(long id, EvidenceVModel evidence)
        {
            if (id != evidence.Id) evidence.Id = id;
            var user = this.GetCurrentUserInfo();

            var currentEvidence = _service.GetById(evidence.Id ?? default);

            if (user.Store.Id != evidence.Transaction.Store.Id) return Forbid();
            var result = _service.UpdateEvidence(evidence);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("/api/transactions/{transactionId}/evidences")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public ActionResult PostEvidence(long transactionId, List<EvidenceVModel> evidenceLst)
        {
            var transactionService = new TransactionService(_context);
            if (transactionService.IsTransactionCanModified(transactionId))
            {
                return Created("",_service.InsertEvidences(evidenceLst));
            }
            return BadRequest(TransactionExceptionMessage.CUR_STATUS_CANNOT_MODIFIED);
        }

        // DELETE: api/Evidences/5
        [HttpDelete("/api/evidences/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public ActionResult DeleteEvidence(long id)
        {
            var current = _service.GetById(id);
            if (current == null) return NotFound();

            var user = this.GetCurrentUserInfo();
            if (user.Store.Id != current.Transaction.Store.Id) return Forbid();

            var transactionService = new TransactionService(_context);
            if (transactionService.IsTransactionCanModified(current.Transaction.Id))
            {
                return Ok(_service.DeleteEvidence(id));
            }
            return BadRequest(TransactionExceptionMessage.CUR_STATUS_CANNOT_MODIFIED);
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
