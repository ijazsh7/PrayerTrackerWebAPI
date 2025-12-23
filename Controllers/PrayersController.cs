using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrayerTrackerWebAPI.Models;

namespace PrayerTrackerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrayersController : ControllerBase
    {
        private readonly PrayerDbContext _context;

        public PrayersController(PrayerDbContext context)
        {
            _context = context;
        }

        // GET: api/Prayers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prayer>>> GetPrayers()
        {
            return await _context.Prayers.ToListAsync();
        }

        // GET: api/Prayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prayer>> GetPrayer(int id)
        {
            var prayer = await _context.Prayers.FindAsync(id);

            if (prayer == null)
            {
                return NotFound();
            }

            return prayer;
        }

        // PUT: api/Prayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrayer(int id, Prayer prayer)
        {
            if (id != prayer.PrayerId)
            {
                return BadRequest();
            }

            _context.Entry(prayer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrayerExists(id))
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

        // POST: api/Prayers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Prayer>> PostPrayer(Prayer prayer)
        {
            _context.Prayers.Add(prayer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrayer", new { id = prayer.PrayerId }, prayer);
        }

        // DELETE: api/Prayers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrayer(int id)
        {
            var prayer = await _context.Prayers.FindAsync(id);
            if (prayer == null)
            {
                return NotFound();
            }

            _context.Prayers.Remove(prayer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrayerExists(int id)
        {
            return _context.Prayers.Any(e => e.PrayerId == id);
        }
    }
}
