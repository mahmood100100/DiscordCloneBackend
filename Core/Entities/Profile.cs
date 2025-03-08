using Microsoft.AspNetCore.Hosting.Server;
using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace DiscordCloneBackend.Core.Entities
{
    public class Profile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string ImageUrl { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual LocalUser User { get; set; } // Navigation property to LocalUser
        [JsonIgnore]
        public ICollection<Server> Servers { get; set; } = new List<Server>();
        [JsonIgnore]
        public ICollection<Member> Members { get; set; } = new List<Member>();
        [JsonIgnore]
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
    }
}
