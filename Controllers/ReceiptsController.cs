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
        [HttpGet("{id}")]
        public async Task<ActionResult<Receipt>> GetReceipt(long id)
        {
            var receipt = await _context.Receipt.FindAsync(id);

            if (receipt == null)
            {
                return NotFound();
            }

            return receipt;
        }

        // PUT: api/Receipts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceipt(long id, Receipt receipt)
        {
            if (id != receipt.Id)
            {
                return BadRequest();
            }

            _context.Entry(receipt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptExists(id))
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

        // POST: api/Receipts
        [HttpPost]
        public async Task<ActionResult<Receipt>> PostReceipt(Receipt receipt)
        {
            _context.Receipt.Add(receipt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReceipt", new { id = receipt.Id }, receipt);
        }

        // DELETE: api/Receipts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Receipt>> DeleteReceipt(long id)
        {
            var receipt = await _context.Receipt.FindAsync(id);
            if (receipt == null)
            {
                return NotFound();
            }

            _context.Receipt.Remove(receipt);
            await _context.SaveChangesAsync();

            return receipt;
        }

        private bool ReceiptExists(long id)
        {
            return _context.Receipt.Any(e => e.Id == id);
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
