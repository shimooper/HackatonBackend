using System.Net;
using System.Net.Mail;
using System.Text;
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
        private readonly SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("recomicrotest@gmail.com", "fqpflaitpwocjgzj"),
            EnableSsl = true,
        };

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
            else if (!Utils.InvalidField(newInteraction.UserName))
            {
                var user = _context.Users.FirstOrDefault(u => u.UserName == newInteraction.UserName);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = newInteraction.UserName,
                        Email = newInteraction.UserName,
                        AddedToDb = DateTime.Now,
                        RealUser = true
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
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
            
            var dbEvent = await _context.Events.FindAsync(dbInteraction.EventId);
            var dbUser = await _context.Users.FindAsync(dbInteraction.UserId);

            if (dbUser.RealUser.Value)
            {
                SendMail(dbEvent, dbUser.Email);
            }

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

        private void SendMail(Event @event, string recipient)
        {
            string startTime1 = @event.StarTime.ToString("yyyyMMddTHHmmssZ");
            string endTime1 = @event.EndTime.ToString("yyyyMMddTHHmmssZ");

            MailMessage msg = new MailMessage();

            msg.From = new MailAddress("recomicrotest@gmail.com", "Reco Smart Calendar");
            msg.To.Add(new MailAddress(recipient));
            msg.Subject = @event.Name;
            msg.Body = @event.Description;

            StringBuilder str = new StringBuilder();
            str.AppendLine("BEGIN:VCALENDAR");

            //PRODID: identifier for the product that created the Calendar object
            str.AppendLine("PRODID:-//ABC Company//Outlook MIMEDIR//EN");
            str.AppendLine("VERSION:2.0");
            str.AppendLine("METHOD:REQUEST");

            str.AppendLine("BEGIN:VEVENT");

            str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", startTime1));//TimeZoneInfo.ConvertTimeToUtc("BeginTime").ToString("yyyyMMddTHHmmssZ")));
            str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
            str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", endTime1));//TimeZoneInfo.ConvertTimeToUtc("EndTime").ToString("yyyyMMddTHHmmssZ")));
            str.AppendLine(string.Format("LOCATION: {0}", @event.Location));

            // UID should be unique.
            str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
            str.AppendLine(string.Format("DESCRIPTION:{0}", msg.Body));
            str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", msg.Body));
            str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));

            str.AppendLine("STATUS:CONFIRMED");
            str.AppendLine("BEGIN:VALARM");
            str.AppendLine("TRIGGER:-PT15M");
            str.AppendLine("ACTION:Accept");
            str.AppendLine("DESCRIPTION:Reminder");
            str.AppendLine("X-MICROSOFT-CDO-BUSYSTATUS:BUSY");
            str.AppendLine("END:VALARM");
            str.AppendLine("END:VEVENT");

            str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));
            str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

            str.AppendLine("END:VCALENDAR");
            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
            ct.Parameters.Add("method", "REQUEST");
            ct.Parameters.Add("name", "meeting.ics");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), ct);
            msg.AlternateViews.Add(avCal);
            //Response.Write(str);// sc.ServicePoint.MaxIdleTime = 2;
            smtpClient.Send(msg);
        }
    }
}