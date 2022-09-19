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
        public async Task<ActionResult<Interaction>> PostInteraction(Interaction interaction)
        {
            if (await _context.Events.FindAsync(interaction.EventId) == null ||
                await _context.Users.FindAsync(interaction.UserId) == null)
            {
                return BadRequest("eventId or userId doesn't exist");
            }

            if (interaction.IsPositive == null)
            {
                return BadRequest("isPositive field must not be null");
            }

            interaction.AddedToDb = DateTime.Now;
            _context.Interactions.Add(interaction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInteraction", new { id = interaction.Id }, interaction);
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