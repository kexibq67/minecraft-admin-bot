using MinecraftAdminBot.Domain.Authorization;

namespace MinecraftAdminBot.Domain.Entities
{
    public class PlayerLink
    {
        public Guid Id { get; set; }
        public long TelegramUserId { get; set; }
        public Guid MinecraftUuid { get; set; }
        public string MinecraftUsername { get; set; } = string.Empty;
        public Guid ServerId { get; set; }
        public Server Server { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    }
}
