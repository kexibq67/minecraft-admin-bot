using MinecraftAdminBot.Domain.Authorization;

namespace MinecraftAdminBot.Domain.Entities
{
    public class LinkCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public Guid ServerId { get; set; }
        public Guid MinecraftUuid { get; set; }
        public string MinecraftUsername { get; set; } = string.Empty;
        public UserRole GrantedRole { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
