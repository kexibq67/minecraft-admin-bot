using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface IPlayerLinkRepository
    {
        Task<PlayerLink?> GetAsync(long telegramUserId, Guid serverId, CancellationToken ct);
        Task<PlayerLink?> GetByUsernameAsync(Guid serverId, string username, CancellationToken ct);
        Task AddAsync(PlayerLink link, CancellationToken ct);
        Task UpdateAsync(PlayerLink link, CancellationToken ct);
        Task DeleteAsync(PlayerLink link, CancellationToken ct);
        Task<IReadOnlyList<PlayerLink>> GetByUserIdAsync(long userId, CancellationToken ct);
        Task<IReadOnlyList<PlayerLink>> GetByServerIdAsync(Guid serverId, CancellationToken ct);
        Task<int> CountAdminsAsync(Guid serverId, CancellationToken ct);
    }
}
