namespace AiCalendarBackend.Models
{
    public sealed class User
    {
        public User()
        {
            Interactions = new HashSet<Interaction>();
        }

        public long Id { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public bool RealUser { get; set; }
        public string? PersonalInterests { get; set; }
        public DateTime AddedToDb { get; set; }
        public ICollection<Interaction> Interactions { get; set; }
    }
}