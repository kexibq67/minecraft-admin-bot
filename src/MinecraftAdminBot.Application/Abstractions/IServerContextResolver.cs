using Telegram.Bot.Types;

namespace MinecraftAdminBot.Application.Abstractions
{
    /// <summary>Определяет, к какому серверу относится входящее сообщение.</summary>
    public interface IServerContextResolver
    {
        Task<Guid?> ResolveServerIdAsync(Update update, CancellationToken ct);
    }
}
