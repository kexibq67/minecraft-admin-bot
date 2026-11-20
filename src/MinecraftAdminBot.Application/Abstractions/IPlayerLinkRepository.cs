using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface IPlayerLinkRepository
    {
        Task<PlayerLink?> GetAsync(long telegramUserId, Guid serverId, CancellationToken ct);
        Task AddAsync(PlayerLink playerLink, CancellationToken ct);
        Task<IReadOnlyList<PlayerLink>> GetByUserIdAsync(long userId, CancellationToken ct);
    }
}
