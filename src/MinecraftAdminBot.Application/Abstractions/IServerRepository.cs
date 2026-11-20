using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface IServerRepository
    {
        Task<Server?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Server?> GetByApiTokenAsync(string apiToken, CancellationToken ct);
        Task<Server?> GetByChatIdAsync(long chatId, CancellationToken ct);
        Task<bool> HasAnyAdminAsync (Guid serverId, CancellationToken ct);
    }
}
