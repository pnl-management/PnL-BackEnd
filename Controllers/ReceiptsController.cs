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
    [Route("api/receipts")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReceiptsController : ControllerBase
    {
        private readonly PLSystemContext _context;
        private readonly IReceiptService _service;

        public ReceiptsController(PLSystemContext context)
        {
            _context = context;
            _service = new ReceiptService(context);
        }

        [HttpGet("/api/receipts/brands/{brandId}")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public ActionResult GetReceiptBrand(int brandId)
        {
            var user = this.GetCurrentUserInfo();
            if (user.Brand.Id != brandId) return Forbid("You cannot access to brand id: " + brandId);
            var result = _service.GetReceiptByBrand(brandId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("/api/receipts/stores/{storeId}")]
        public ActionResult GetReceiptStore(int storeId)
        {
            var user = this.GetCurrentUserInfo();
            var role = user.Role;

            switch (role ?? -100)
            {
                case ParticipantsRoleConst.ACCOUNTANT_ID:
                case ParticipantsRoleConst.INVESTOR_ID:
                    StoreService storeService = new StoreService(_context, null);
                    var brand = storeService.GetBrandOfStore(storeId);
                    if (brand == null) return BadRequest("Cannot find brand of store");

                    if (brand.Id != user.Brand.Id) return Forbid("You cannot access to brand ID: " + brand.Id);
                    break;
                case ParticipantsRoleConst.STORE_MANAGER_ID:
                    if (user.Store.Id != storeId) return Forbid("You cannot access to store ID: " + storeId);
                    break;
            }
            return Ok(_service.GetReceiptByStore(storeId));
        }

        // GET: api/Receipts/5
        [HttpGet("/api/receipts/{id}")]
        public ActionResult GetReceipt(long id)
        {
            var receipt = _service.GetById(id);

            var user = this.GetCurrentUserInfo();
            var role = user.Role;

            switch (role ?? -100)
            {
                case ParticipantsRoleConst.ACCOUNTANT_ID:
                case ParticipantsRoleConst.INVESTOR_ID:
                    StoreService storeService = new StoreService(_context, null);
                    var brand = storeService.GetBrandOfStore(receipt.Store.Id ?? -100);
                    if (brand == null) return BadRequest("Cannot find brand of store");

                    if (brand.Id != user.Brand.Id) return Forbid("You cannot access to brand ID: " + brand.Id);
                    break;
                case ParticipantsRoleConst.STORE_MANAGER_ID:
                    if (user.Store.Id != receipt.Store.Id) return Forbid("You cannot access to store ID: " + receipt.Store.Id);
                    break;
            }
            return Ok(receipt);
        }

        [HttpPut("/api/receipts/{id}/judge")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT)]
        public ActionResult JudgeReceipt(long id, String type)
        {
            var current = _service.GetById(id);

            if (current == null) return NotFound("Not found Receipt ID: " + id);

            var user = this.GetCurrentUserInfo();

            if (current.Brand.Id != user.Brand.Id) return Forbid("You cannot access to Brand ID: " + user.Brand.Id);

            if (_service.JudgeReceipt(id, type)) return Ok();
            return BadRequest("Cannot judge receipt");
        }

        [HttpPut("/api/receipts/{id}")]
        [Authorize(Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public IActionResult PutReceipt(long id, ReceiptVModel receipt)
        {
            receipt.Id = id;

            var user = this.GetCurrentUserInfo();

            var current = _service.GetById(receipt.Id ?? -100);

            if (current == null) return NotFound("Not found receipt ID: " + receipt.Id);

            if (current.Store.Id != user.Store.Id) return Forbid("You cannot update receipt of Store ID: " + current.Store.Id);

            receipt.LastModifiedBy = new ParticipantVModel() { Id = user.Id };

            var result = _service.Update(receipt);

            if (result == null) return BadRequest("Cannot Update receipt");

            return Ok(result);
        }

        [HttpPost("/api/receipts")]
        [Authorize(Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public ActionResult PostReceipt(ReceiptVModel receipt)
        {
            var user = this.GetCurrentUserInfo();

            receipt.CreateBy = new ParticipantVModel() { Id = user.Id };
            receipt.Status = ReceiptStatusConst.CREATED;
            receipt.Brand = new BrandVModel() { Id = user.Brand.Id };
            receipt.Store = new StoreVModel() { Id = user.Store.Id };

            var result = _service.Create(receipt);
            if (result == null) return BadRequest("Cannot create receipt");

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
