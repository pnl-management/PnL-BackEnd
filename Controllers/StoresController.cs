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
    public class StoresController : ControllerBase
    {
        private readonly PLSystemContext _context;
        private readonly IStoreService _service;

        public StoresController(PLSystemContext context)
        {
            _context = context;
            _service = new StoreService(context);
        }

        // GET: api/brands/stores
        [HttpGet]
        [Route("/api/brands/stores")]
        public ActionResult<IEnumerable<Store>> GetStore(string sort, string filter, string query, int offset, int limit)
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

            result = _service.QueryByBrand(query, sort, brandId, offset, limit);

            result = _service.FilterColumns(filter, (IEnumerable<StoreVModel>)result);

            return Ok(result);
        }

        // GET: api/brands/stores
        [HttpGet]
        [Route("/api/brands/stores/length")]
        public ActionResult<IEnumerable<Store>> GetStoreCount(string query)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string participantIdVal = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            long userId;

            long.TryParse(participantIdVal, out userId);

            IParticipantService participantService = new ParticipantService(_context);

            int brandId = participantService.FindByUserId(userId).Brand.Id;
            var result = _service.CountQueryList(query, brandId);
            return Ok(result);
        }

        // GET: api/Stores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Store>> GetStore(int id)
        {
            var store = await _context.Store.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return store;
        }

        // PUT: api/Stores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStore(int id, Store store)
        {
            if (id != store.Id)
            {
                return BadRequest();
            }

            _context.Entry(store).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(id))
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

        // POST: api/Stores
        [HttpPost]
        public async Task<ActionResult<Store>> PostStore(Store store)
        {
            _context.Store.Add(store);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStore", new { id = store.Id }, store);
        }

        // DELETE: api/Stores/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Store>> DeleteStore(int id)
        {
            var store = await _context.Store.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            _context.Store.Remove(store);
            await _context.SaveChangesAsync();

            return store;
        }

        private bool StoreExists(int id)
        {
            return _context.Store.Any(e => e.Id == id);
        }
    }
}
