using System.Text.Json.Serialization;

namespace Eshop.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }

        [JsonIgnore]
        public string? Password { get; set; }

        public List<UserRole> Roles { get; set; } = new List<UserRole>();
        public bool Active { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}