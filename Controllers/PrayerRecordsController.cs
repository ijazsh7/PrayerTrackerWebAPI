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
    public class PrayerRecordsController : ControllerBase
    {
        private readonly PrayerDbContext _context;

        public PrayerRecordsController(PrayerDbContext context)
        {
            _context = context;
        }

        // GET: api/PrayerRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrayerRecord>>> GetPrayerRecords()
        {
            return await _context.PrayerRecords.ToListAsync();
        }

        // GET: api/PrayerRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrayerRecord>> GetPrayerRecord(int id)
        {
            var prayerRecord = await _context.PrayerRecords.FindAsync(id);

            if (prayerRecord == null)
            {
                return NotFound();
            }

            return prayerRecord;
        }

        // GET: api/PrayerRecords/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<PrayerRecord>>> GetPrayerRecordsByUserId(int userId)
        {
            // Retrieve prayer records for a specific user

            /*
            var prayerRecords = await _context.PrayerRecords
                                              .Where(pr => pr.UserId == userId)  // Ensure you filter by UserId
                                              .ToListAsync();

            if (prayerRecords == null || !prayerRecords.Any())
            {
                return NotFound(new { message = "No prayer records found for this user." });
            }

            return Ok(prayerRecords);
            */


            var prayerRecords = await _context.PrayerRecords
                                      .Include(pr => pr.Prayer) // Load related Prayer entity
                                      .Where(pr => pr.UserId == userId)
                                      .Select(pr => new
                                      {
                                          pr.PrayerRecordId,
                                          pr.UserId,
                                          pr.PrayerId,
                                          PrayerName = pr.Prayer.Name, // Assuming Prayer table has a "Name" column
                                          pr.Status,
                                          pr.RecordedAt
                                      })
                                      .ToListAsync();

            if (prayerRecords == null || !prayerRecords.Any())
            {
                return NotFound(new { message = "No prayer records found for this user." });
            }

            return Ok(prayerRecords);
        }


        // GET: api/PrayerRecords/daily/{userId}
        [HttpGet("daily/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetDailyPrayerRecordsByUserId(int userId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date); // Adjust to local timezone if needed

            var prayerRecords = await _context.PrayerRecords
                .Include(pr => pr.Prayer)
                .Where(pr => pr.UserId == userId &&
                             DateOnly.FromDateTime((DateTime)pr.RecordedAt) == today)
                .Select(pr => new
                {
                    pr.PrayerRecordId,
                    pr.UserId,
                    pr.PrayerId,
                    PrayerName = pr.Prayer.Name,
                    pr.Status,
                    pr.RecordedAt
                })
                .ToListAsync();

            if (!prayerRecords.Any())
            {
                return NotFound(new { message = "No prayer records found for today." });
            }

            return Ok(prayerRecords);
        }


        [HttpGet("userprayerssummary/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetPrayerSummaryByUserId(int userId)
        {
            var summary = await _context.PrayerRecords
                .Where(pr => pr.UserId == userId)
                .GroupBy(pr => new { pr.PrayerId, pr.Status })
                .Select(g => new
                {
                    PrayerId = g.Key.PrayerId,
                    PrayerName = g.Select(p => p.Prayer.Name).FirstOrDefault(),
                    Status = g.Key.Status,
                    Count = g.Count()
                })
                .OrderBy(result => result.PrayerId)
                .ToListAsync();

            if (!summary.Any())
            {
                return NotFound(new { message = "No prayer records found for this user." });
            }

            return Ok(summary);
        }




        // GET: api/PrayerRecords/user/{userName}
        [HttpGet("userprayers/{userName}")]
        public async Task<ActionResult<IEnumerable<PrayerRecord>>> GetPrayerRecordsByUserId(string userName)
        {
            // Retrieve prayer records for a specific user
            var prayerRecords = await _context.PrayerRecords
                                              .Where(pr => pr.User.Username == userName)  // Ensure you filter by UserId
                                              .ToListAsync();

            if (prayerRecords == null || !prayerRecords.Any())
            {
                return NotFound(new { message = "No prayer records found for this user." });
            }

            return Ok(prayerRecords);
        }


        // PUT: api/PrayerRecords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrayerRecord(int id, PrayerRecord prayerRecord)
        {
            if (id != prayerRecord.PrayerRecordId)
            {
                return BadRequest();
            }

            _context.Entry(prayerRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrayerRecordExists(id))
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

        // POST: api/PrayerRecords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PrayerRecord>> PostPrayerRecord(PrayerRecord prayerRecord)
        {
            _context.PrayerRecords.Add(prayerRecord);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrayerRecord", new { id = prayerRecord.PrayerRecordId }, prayerRecord);
        }

        // DELETE: api/PrayerRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrayerRecord(int id)
        {
            var prayerRecord = await _context.PrayerRecords.FindAsync(id);
            if (prayerRecord == null)
            {
                return NotFound();
            }

            _context.PrayerRecords.Remove(prayerRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        // POST: api/PrayerRecords/markprayer
        [HttpPost("markprayer")]
        public async Task<ActionResult<PrayerRecord>> CreateOrUpdatePrayerRecord([FromBody] PrayerRecordDTO model)
        {
            // Validate the status
            if (model.Status != "Prayed" && model.Status != "Missed")
            {
                return BadRequest(new { message = "Invalid status. The status must be 'Prayed' or 'Missed'." });
            }

            // Check if the prayer record already exists for the user and prayer
            var existingRecord = await _context.PrayerRecords
                .FirstOrDefaultAsync(pr => pr.UserId == model.UserId && pr.PrayerId == model.PrayerId);

            if (existingRecord != null)
            {
                // If record exists, update the existing record
                existingRecord.Status = model.Status;
                existingRecord.RecordedAt = DateTime.Now;

                // Update the prayer record in the database
                _context.PrayerRecords.Update(existingRecord);
                await _context.SaveChangesAsync();

                // Return the updated prayer record
                return Ok(existingRecord);
            }
            else
            {
                // If record does not exist, create a new prayer record
                var prayerRecord = new PrayerRecord
                {
                    UserId = model.UserId,
                    PrayerId = model.PrayerId,
                    RecordedAt = DateTime.Now,
                    Status = model.Status
                };

                // Add the new prayer record to the database
                _context.PrayerRecords.Add(prayerRecord);
                await _context.SaveChangesAsync();

                // Return the created prayer record
                return CreatedAtAction(nameof(GetPrayerRecord), new { id = prayerRecord.PrayerRecordId }, prayerRecord);
            }
        }


        private bool PrayerRecordExists(int id)
        {
            return _context.PrayerRecords.Any(e => e.PrayerRecordId == id);
        }
    }

    public class PrayerRecordDTO
    {
        public int UserId { get; set; }
        public int PrayerId { get; set; }  // E.g., Fajr, Dhuhr, Asr, Maghrib, Isha       
        public string Status { get; set; }  // "Prayed" or "Missed"
    }

}
