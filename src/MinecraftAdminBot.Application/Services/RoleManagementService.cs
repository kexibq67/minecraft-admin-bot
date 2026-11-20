using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;
using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Services
{
    public class RoleManagementService : IRoleManagementService
    {
        private readonly IPlayerLinkRepository _links;
        public RoleManagementService(IPlayerLinkRepository links) => _links = links;

        public async Task<RoleOperationResult> SetRoleAsync(Guid serverId, long actorUserId, string targetUsername, UserRole newRole, CancellationToken ct)
        {
            var actor = await _links.GetAsync(actorUserId, serverId, ct);
            if (actor is null) return new(false, "Вы не привязаны к этому серверу.");

            if (newRole > actor.Role) return new(false, $"Нельзя выдать роль выше своей собственной ({actor.Role}).");

            var target = await _links.GetByUsernameAsync(serverId, targetUsername, ct);
            if (target is null) return new(false, $"Игрок `{targetUsername}` не найден на этом сервере.");

            if (target.Id == actor.Id && newRole < actor.Role) return new(false, "Нельзя понизить собственную роль.");

            if (target.Role == UserRole.Admin && newRole != UserRole.Admin)
            {
                var adminCount = await _links.CountAdminsAsync(serverId, ct);
                if (adminCount <= 1) return new(false, "Нельзя понизить последнего администратора сервера.");
            }

            if (target.Role == newRole) return new(false, $"У игрока `{targetUsername}` уже роль {newRole}");

            var oldRole = target.Role;
            target.Role = newRole;
            await _links.UpdateAsync(target, ct);

            return new(true, $"\"✅ `{{targetUsername}}`: {{oldRole}} → **{{newRole}}**.");
        }

        public async Task<RoleOperationResult> UnlinkPlayerAsync(Guid serverId, long actorUserId, string targetUsername, CancellationToken ct)
        {
            var target = await _links.GetByUsernameAsync(serverId, targetUsername, ct);
            if (target is null) return new(false, $"Игрок `{targetUsername}` не найден на этом сервере.");
            if (target.TelegramUserId == actorUserId) return new(false, "Нельзя отвязать самого себя. Попросите другого администратора.");

            if (target.Role == UserRole.Admin)
            {
                var adminCount = await _links.CountAdminsAsync(serverId, ct);
                if (adminCount <= 1) return new(false, "Нельзя отвязать последнего администратора сервера.");
            }

            await _links.DeleteAsync(target, ct);
            return new(true, $"✅ `{targetUsername}` отвязан от сервера.");
        }

        public Task<IReadOnlyList<PlayerLink>> GetPlayersAsync(Guid serverId, CancellationToken ct) => _links.GetByServerIdAsync(serverId, ct);
    }
}
