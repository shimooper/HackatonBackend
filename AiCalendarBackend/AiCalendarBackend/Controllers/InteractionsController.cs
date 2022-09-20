using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiCalendarBackend.Models;

namespace AiCalendarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InteractionsController : ControllerBase
    {
        private readonly CalendarContext _context;

        public InteractionsController(CalendarContext context)
        {
            _context = context;
        }

        // GET: api/Interactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Interaction>>> GetInteractions()
        {
            return await _context.Interactions.ToListAsync();
        }

        // GET: api/Interactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Interaction>> GetInteraction(long id)
        {
            var interaction = await _context.Interactions.FindAsync(id);

            if (interaction == null)
            {
                return NotFound();
            }

            return interaction;
        }

        // PUT: api/Interactions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInteraction(long id, Interaction interaction)
        {
            if (id != interaction.Id)
            {
                return BadRequest();
            }

            if (await _context.Events.FindAsync(interaction.EventId) == null ||
                await _context.Users.FindAsync(interaction.UserId) == null)
            {
                return BadRequest("eventId or userId doesn't exist");
            }

            if (interaction.IsPositive == null)
            {
                return BadRequest("isPositive field must not be null");
            }

            _context.Entry(interaction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InteractionExists(id))
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

        // POST: api/Interactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Interaction>> PostInteraction(NewInteraction newInteraction)
        {
            if (await _context.Events.FindAsync(newInteraction.EventId) == null)
            {
                return BadRequest("eventId doesn't exist");
            }

            if (newInteraction.IsPositive == null)
            {
                return BadRequest("isPositive field must not be null");
            }

            if (newInteraction.UserId != null)
            {
                if (await _context.Users.FindAsync(newInteraction.UserId) == null)
                {
                    return BadRequest("userId doesn't exist");
                }
            }
            else if (Utils.InvalidField(newInteraction.UserName))
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == newInteraction.UserName);
                if (user == null)
                {
                    return BadRequest("userName doesn't exist");
                }

                newInteraction.UserId = user.Id;
            }
            else
            {
                return BadRequest("userId or userName must be supplied");
            }

            var dbInteraction = _context.Interactions.FirstOrDefault(i =>
                i.EventId == newInteraction.EventId && i.UserId == newInteraction.UserId);
            if (dbInteraction != null)
            {
                dbInteraction.IsPositive = newInteraction.IsPositive;
                dbInteraction.AddedToDb = DateTime.Now;
                _context.Entry(dbInteraction).State = EntityState.Modified;
            }
            else
            {
                dbInteraction = new Interaction
                {
                    EventId = newInteraction.EventId,
                    UserId = newInteraction.UserId.Value,
                    IsPositive = newInteraction.IsPositive,
                    AddedToDb = DateTime.Now
                };

                _context.Interactions.Add(dbInteraction);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInteraction", new { id = dbInteraction.Id }, dbInteraction);
        }

        // DELETE: api/Interactions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInteraction(long id)
        {
            var interaction = await _context.Interactions.FindAsync(id);
            if (interaction == null)
            {
                return NotFound();
            }

            _context.Interactions.Remove(interaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InteractionExists(long id)
        {
            return (_context.Interactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}