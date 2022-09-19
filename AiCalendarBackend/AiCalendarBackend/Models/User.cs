using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AiCalendarBackend.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
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

        [JsonIgnore]
        public virtual ICollection<Interaction> Interactions { get; set; }
    }


    public class UserForLeaderBoard
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
    }
}