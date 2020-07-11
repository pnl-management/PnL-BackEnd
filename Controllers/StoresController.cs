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
using Microsoft.Extensions.Caching.Distributed;
using PnLReporter.EnumInfo;
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
        private readonly IDistributedCache _distributedCache;

        public StoresController(PLSystemContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _service = new StoreService(context, distributedCache);
            _distributedCache = distributedCache;
        }

        // GET: api/brands/stores
        [HttpGet]
        [Route("/api/brands/stores")]
        public ActionResult<IEnumerable<Store>> GetStore(string sort, string filter, string query, int offset, int limit)
        {
            var user = this.GetCurrentUserInfo();

            int brandId = user.Brand.Id;

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
            var user = this.GetCurrentUserInfo();

            int brandId = user.Brand.Id;
            var result = _service.CountQueryList(query, brandId);
            return Ok(result);
        }

        /*[HttpGet("/test")]
        public ActionResult Test()
        {
            var cacheKey = "TheTime";
            var currentTime = DateTime.Now.ToString();
            var cachedTime = _distributedCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(cachedTime))
            {
                // cachedTime = "Expired";
                // Cache expire trong 30s
                var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
                // Nạp lại giá trị mới cho cache
                _distributedCache.SetString(cacheKey, currentTime, options);
                cachedTime = _distributedCache.GetString(cacheKey);
            }
            var result = $"Current Time : {currentTime} \nCached  Time : {cachedTime}";
            return Ok(result);
        }*/

        [HttpGet("/api/stores/{id}")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public ActionResult GetStore(int id)
        {
            System.Diagnostics.Debug.WriteLine("SomeText");
            var store = _service.GetById(id);
            if (store == null) return NotFound();

            var user = this.GetCurrentUserInfo();

            if (user.Brand.Id != store.Brand.Id) return Forbid();

            store.Brand = user.Brand;

            return Ok(store);
        }

        /*[HttpDelete("/api/stores-cache/{id}")]
        public ActionResult ClearCache(int id)
        {
            _distributedCache.Remove(id + "");
            return Ok();
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
        }*/

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
