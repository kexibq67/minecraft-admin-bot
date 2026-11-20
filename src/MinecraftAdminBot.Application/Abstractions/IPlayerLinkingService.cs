using MinecraftAdminBot.Domain.Authorization;
using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface IPlayerLinkingService
    {
        Task<LinkCode> GenerateLinkCodeAsync(Guid serverId, Guid playerUuid, string playerName, CancellationToken ct);

        Task<LinkResult> CompleteLinkAsync(long telegramUserId, string code, CancellationToken ct);
    }

    public record LinkResult(bool Success, string Message, Guid? ServerId, UserRole? Role);
}
