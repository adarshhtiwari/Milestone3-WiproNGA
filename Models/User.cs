
namespace SecureNoteTakingApi.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}