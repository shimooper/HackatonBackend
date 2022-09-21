﻿using System.Net;
using System.Net.Mail;
using System.Text;
using AiCalendarBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AiCalendarBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CalendarContext _context;

        public UsersController(CalendarContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/username/yairshsh
        [HttpGet("username/{userName}")]
        public async Task<ActionResult<User>> GetUser(string userName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // PUT: api/Users/username/Yair
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("username/{username}")]
        public async Task<IActionResult> PutUserByUserName(string userName, User user)
        {
            if (userName != user.UserName)
            {
                return BadRequest();
            }

            var dbUser = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if (dbUser == null)
            {
                user.AddedToDb = DateTime.Now;

                if (Utils.InvalidField(user.Email))
                {
                    user.Email = user.UserName;
                }
                
                _context.Users.Add(user);
            }
            else
            {
                dbUser.PersonalInterests = user.PersonalInterests;
                _context.Entry(dbUser).State = EntityState.Modified;
            }
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (Utils.InvalidField(user.UserName))
            {
                return BadRequest("UserName must not be empty");
            }

            if (user.RealUser == null)
            {
                return BadRequest("RealUser must not be null");
            }

            user.AddedToDb = DateTime.Now;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Users/Yair/recommendations
        [HttpGet("{userName}/recommendations")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsRecommendationsForUser(string userName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if (user == null)
            {
                user = new User
                {
                    UserName = userName,
                    Email = userName,
                    AddedToDb = DateTime.Now,
                    RealUser = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var recommendations = await GetRecommendations();
            return recommendations;
        }

        // GET: api/Users/leaderboard
        [HttpGet("leaderboard")]
        public async Task<ActionResult<IEnumerable<UserForLeaderBoard>>> GetLeaderBoard()
        {
            var users = await _context.Users.Include(user => user.Interactions)
                .OrderByDescending(user => user.Interactions.Count)
                .Take(5)
                .Select(user => new UserForLeaderBoard
                    { Id = user.Id, UserName = user.UserName, Score = user.Interactions.Count * 10 }).ToListAsync();

            return users;
        }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<List<Event>> GetRecommendations()
        {
            // Currently return random events. TODO - call Reco service
            var eventsCount = await _context.Events.CountAsync();
            var toSkip = new Random().Next(0, eventsCount);
            var recommendations = await _context.Events.Skip(toSkip).Take(5).ToListAsync();

            return recommendations;
        }
    }
}