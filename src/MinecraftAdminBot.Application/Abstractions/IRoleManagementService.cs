using MinecraftAdminBot.Domain.Authorization;
using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface IRoleManagementService
    {
        Task<RoleOperationResult> SetRoleAsync(Guid serverId, long actorUserId, string targetUsername, UserRole newRole, CancellationToken ct);
        Task<RoleOperationResult> UnlinkPlayerAsync(Guid serverId, long actorUserId, string targetUsername, CancellationToken ct);
        Task<IReadOnlyList<PlayerLink>> GetPlayersAsync(Guid serverId, CancellationToken ct);
    }

    public record RoleOperationResult(bool Success, string Message);
}
