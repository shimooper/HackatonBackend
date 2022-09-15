using Microsoft.EntityFrameworkCore;

namespace AiCalendarBackend.Models
{
    public class CalendarContext : DbContext
    {
        public CalendarContext(DbContextOptions<CalendarContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Interaction> Interactions { get; set; } = null!;
    }
}