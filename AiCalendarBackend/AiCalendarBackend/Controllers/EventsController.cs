using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AiCalendarBackend.Models;

namespace AiCalendarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly CalendarContext _context;
        private readonly Random _gen = new Random();

        private readonly List<string> _locations = new List<string>()
        {
            "Tel-Aviv", "Jerusalem", "Kfar-Saba", "Eilat", "Haifa", "Beer-Sheva", "Nazareth", "Herzliya", "Ramat-Gan"
        };

        public EventsController(CalendarContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/Events/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(long id)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }
        
        // GET: api/Events/random
        [HttpGet("random")]
        public async Task<ActionResult<Event>> GetRandomEvent()
        {
            var eventsCount = await _context.Events.CountAsync();
            var toSkip = new Random().Next(0, eventsCount);

            return _context.Events.Skip(toSkip).First();
        }

        // PUT: api/Events/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(long id, Event @event)
        {
            if (id != @event.Id)
            {
                return BadRequest();
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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

        // POST: api/Events
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(Event @event)
        {
            if (string.IsNullOrEmpty(@event.Name) || string.IsNullOrEmpty(@event.Description) ||
                string.IsNullOrEmpty(@event.Tags) || string.IsNullOrEmpty(@event.Language))
            {
                return BadRequest("Fields must not be empty");
            }

            if (_context.Events.FirstOrDefault(e => 
                    e.Name == @event.Name && e.StarTime == @event.StarTime) != null)
            {
                return BadRequest("Event already exists");
            }

            if (@event.StarTime == default || @event.EndTime == default)
            {
                var day = _gen.Next(1, 30);
                var hour = _gen.Next(10, 20);
                var minute = _gen.Next(0, 59);
                var durationHours = _gen.Next(1, 4);

                @event.StarTime = new DateTime(2022, 10, day, hour, minute, 0);
                @event.EndTime = @event.StarTime + TimeSpan.FromHours(durationHours);
            }

            if (string.IsNullOrEmpty(@event.Location))
            {
                var index = _gen.Next(_locations.Count);
                @event.Location = _locations[index];
            }

            @event.AddedToDb = DateTime.Now;
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(long id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(long id)
        {
            return (_context.Events?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}