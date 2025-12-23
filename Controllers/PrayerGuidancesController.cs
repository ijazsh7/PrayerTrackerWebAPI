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
    public class PrayerGuidancesController : ControllerBase
    {
        private readonly PrayerDbContext _context;

        public PrayerGuidancesController(PrayerDbContext context)
        {
            _context = context;
        }

        // GET: api/PrayerGuidances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrayerGuidance>>> GetPrayerGuidances()
        {
            return await _context.PrayerGuidances.ToListAsync();
        }

        // GET: api/PrayerGuidances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrayerGuidance>> GetPrayerGuidance(int id)
        {
            var prayerGuidance = await _context.PrayerGuidances.FindAsync(id);

            if (prayerGuidance == null)
            {
                return NotFound();
            }

            return prayerGuidance;
        }

        // PUT: api/PrayerGuidances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrayerGuidance(int id, PrayerGuidance prayerGuidance)
        {
            if (id != prayerGuidance.GuidanceId)
            {
                return BadRequest();
            }

            _context.Entry(prayerGuidance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrayerGuidanceExists(id))
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

        // POST: api/PrayerGuidances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PrayerGuidance>> PostPrayerGuidance(PrayerGuidance prayerGuidance)
        {
            _context.PrayerGuidances.Add(prayerGuidance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrayerGuidance", new { id = prayerGuidance.GuidanceId }, prayerGuidance);
        }

        // DELETE: api/PrayerGuidances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrayerGuidance(int id)
        {
            var prayerGuidance = await _context.PrayerGuidances.FindAsync(id);
            if (prayerGuidance == null)
            {
                return NotFound();
            }

            _context.PrayerGuidances.Remove(prayerGuidance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrayerGuidanceExists(int id)
        {
            return _context.PrayerGuidances.Any(e => e.GuidanceId == id);
        }
    }
}
