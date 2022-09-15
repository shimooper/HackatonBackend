namespace AiCalendarBackend.Models
{
    public class Event
    {
        public Event(long id, string? name, string? description, string? location, DateTime starTime, DateTime endTime, float price, string? tags, string? language)
        {
            Id = id;
            Name = name;
            Description = description;
            Location = location;
            StarTime = starTime;
            EndTime = endTime;
            Price = price;
            Tags = tags;
            Language = language;
        }

        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
        public float Price { get; set; }
        public string? Tags { get; set; }
        public string? Language { get; set; }
    }
}