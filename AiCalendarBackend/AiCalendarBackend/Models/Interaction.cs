using Newtonsoft.Json;

namespace AiCalendarBackend.Models
{
    public class Interaction
    {
        public long Id { get; set; }

        public long EventId { get; set; }
        public long UserId { get; set; }

        [JsonIgnore]
        public virtual Event? Event { get; set; }
        [JsonIgnore]
        public virtual User? User { get; set; }

        public bool? IsPositive { get; set; }
        public DateTime AddedToDb { get; set; }
    }

    public class NewInteraction
    {
        public long EventId { get; set; }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public bool? IsPositive { get; set; }
    }
}