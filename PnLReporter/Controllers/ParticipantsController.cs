using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using PnLReporter.Models;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        private readonly PLSystemContext _context;

        public ParticipantsController(PLSystemContext context)
        {
            _context = context;
        }

        // GET: api/Participants
        [HttpGet]
        public async Task<UserRecord> GetParticipant()
        {
            System.Diagnostics.Debug.WriteLine("\n\n\n\n\nahihihihihi\n" + HeaderNames.Authorization);
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(Request.Headers[HeaderNames.Authorization]);
            string uid = decodedToken.Uid;
            UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            return userRecord;
        }

        // GET: api/Participants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Participant>> GetParticipant(string id)
        {
            var participant = await _context.Participant.FindAsync(id);

            if (participant == null)
            {
                return NotFound();
            }

            return participant;
        }

        // PUT: api/Participants/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParticipant(string id, Participant participant)
        {
            if (id != participant.Username)
            {
                return BadRequest();
            }

            _context.Entry(participant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParticipantExists(id))
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

        // POST: api/Participants
        [HttpPost]
        public async Task<ActionResult<Participant>> PostParticipant(Participant participant)
        {
            _context.Participant.Add(participant);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ParticipantExists(participant.Username))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetParticipant", new { id = participant.Username }, participant);
        }

        // DELETE: api/Participants/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Participant>> DeleteParticipant(string id)
        {
            var participant = await _context.Participant.FindAsync(id);
            if (participant == null)
            {
                return NotFound();
            }

            _context.Participant.Remove(participant);
            await _context.SaveChangesAsync();

            return participant;
        }

        private bool ParticipantExists(string id)
        {
            return _context.Participant.Any(e => e.Username == id);
        }
    }
}
