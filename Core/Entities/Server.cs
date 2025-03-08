using System.Text.Json.Serialization;
using System.Threading.Channels;

namespace DiscordCloneBackend.Core.Entities
{
    public class Server
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string InviteCode { get; set; } = Guid.NewGuid().ToString();
        public string ProfileId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [JsonIgnore]
        public Profile Profile { get; set; }
        [JsonIgnore]
        public ICollection<Member> Members { get; set; } = new List<Member>();
        [JsonIgnore]
        public ICollection<Channel> Channels { get; set; } = new List<Channel>();
    }
}
