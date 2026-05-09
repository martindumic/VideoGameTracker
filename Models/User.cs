using System.ComponentModel.DataAnnotations;

namespace VideoGameTracker.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public DateTime RegisteredAt { get; set; }

        public virtual ICollection<GameEntry> GameEntries { get; set; }

        public User()
        {
            GameEntries = new List<GameEntry>();
        }
    }
}
