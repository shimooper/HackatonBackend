namespace AiCalendarBackend.Models
{
    public class Interaction
    {
        public long Id { get; set; }

        public long EventId { get; set; }
        public long UserId { get; set; }

        public virtual Event? Event { get; set; }
        public virtual User? User { get; set; }

        public bool IsPositive { get; set; }
        public DateTime AddedToDb { get; set; }
    }
}