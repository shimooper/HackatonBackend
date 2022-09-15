namespace AiCalendarBackend.Models
{
    public sealed class Event
    {
        public Event()
        {
            Interactions = new HashSet<Interaction>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime EndTime { get; set; }
        public float Price { get; set; }
        public string Tags { get; set; }
        public string Language { get; set; }
        public DateTime AddedToDb { get; set; }
        public ICollection<Interaction> Interactions { get; set; }
    }
}