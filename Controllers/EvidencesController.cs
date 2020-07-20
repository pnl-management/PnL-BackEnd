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

        [HttpGet("/api/receipts/{receiptId}/evidences")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetEvidenceLst(long receiptId)
        {
            var user = this.GetCurrentUserInfo();
            bool isValid = false;
            var receipt = (new ReceiptService(_context)).GetById(receiptId);

            if (receipt == null) return NotFound();

            switch (user.Role?? default)
            {
                case ParticipantsRoleConst.ACCOUNTANT_ID:
                case ParticipantsRoleConst.INVESTOR_ID:
                    if (receipt.Brand.Id != user.Brand.Id)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                    
                    break;
                case ParticipantsRoleConst.STORE_MANAGER_ID:
                    if (receipt.Store.Id != user.Store.Id)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                    break;
            }

            if (isValid)
            {
                return Ok(_service.GetListEvidenceOfReceipt(receiptId));
            }

            return Forbid();
        }

        [HttpGet("/api/evidences/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult GetEvidence(long id)
        {
            var result = _service.GetById(id);
            if (result == null) return NotFound();

            var user = this.GetCurrentUserInfo();

            if (user.Brand.Id != result.Receipt.Brand.Id) return Forbid();

            return Ok(result);
        }

        [HttpPut("/api/evidences/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public IActionResult PutEvidence(long id, EvidenceVModel evidence)
        {
            if (id != evidence.Id) evidence.Id = id;
            var user = this.GetCurrentUserInfo();

            var currentEvidence = _service.GetById(evidence.Id ?? default);

            if (user.Store.Id != currentEvidence.Receipt.Store.Id) return Forbid();
            var result = _service.UpdateEvidence(evidence);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("/api/receipts/{receiptId}/evidences")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public ActionResult PostEvidence(long receiptId, List<EvidenceVModel> evidenceLst)
        {
            var receiptService = new ReceiptService(_context);
            if (receiptService.IsReceiptCanModified(receiptId))
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
            if (user.Store.Id != current.Receipt.Store.Id) return Forbid();

            var receiptService = new ReceiptService(_context);
            if (receiptService.IsReceiptCanModified(current.Receipt.Id ?? 0))
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
