using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PnLReporter.Models;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly PLSystemContext _context;

        public BrandsController(PLSystemContext context)
        {
            _context = context;
        }

        // GET: api/Brands
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Brand>>> GetBrand()
        {
            return await _context.Brand.ToListAsync();
        }

        // GET: api/Brands/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Brand>> GetBrand(string id)
        {
            var brand = await _context.Brand.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            return brand;
        }

        // PUT: api/Brands/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBrand(string id, Brand brand)
        {
            if (id != brand.BrandId)
            {
                return BadRequest();
            }
            var currBrand = await _context.Brand.FindAsync(id);
            currBrand.Name = brand.Name;
            currBrand.Status = brand.Status;
            _context.Entry(currBrand).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BrandExists(id))
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

        // POST: api/Brands
        [HttpPost]
        public async Task<ActionResult<Brand>> PostBrand(Brand brand)
        {
            brand.CreatedTime = DateTime.Now;
            _context.Brand.Add(brand);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BrandExists(brand.BrandId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBrand", new { id = brand.BrandId }, brand);
        }

        // DELETE: api/Brands/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Brand>> DeleteBrand(string id)
        {
            var brand = await _context.Brand.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brand.Remove(brand);
            await _context.SaveChangesAsync();

            return brand;
        }

        private bool BrandExists(string id)
        {
            return _context.Brand.Any(e => e.BrandId == id);
        }
    }
}
