using MinecraftAdminBot.Application.Abstractions;
using Telegram.Bot.Types;

namespace MinecraftAdminBot.Infrastructure.Telegram
{
    /// <summary>Определяет сервер по ChatId, к которому привязан чат.</summary>
    public class ChatServerContextResolver : IServerContextResolver
    {
        private readonly IServerRepository _servers;
        public ChatServerContextResolver(IServerRepository servers) => _servers = servers;

        public async Task<Guid?> ResolveServerIdAsync(Update update, CancellationToken ct)
        {
            if (update.Message?.Chat.Id is not { } chatId) return null;
            var server = await _servers.GetByChatIdAsync(chatId, ct);
            return server?.Id;
        }
    }
}
