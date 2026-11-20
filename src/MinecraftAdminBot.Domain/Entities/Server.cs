namespace MinecraftAdminBot.Domain.Entities
{
    public class Server
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ApiToken { get; set; } = string.Empty;    // для аутентификации мода
        public long? TelegramChatId { get; set; }               // чат, привязанный к серверу
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
