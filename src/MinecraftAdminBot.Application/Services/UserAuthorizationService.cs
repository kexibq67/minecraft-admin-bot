using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;

namespace MinecraftAdminBot.Application.Services
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly IPlayerLinkRepository _links;

        public UserAuthorizationService(IPlayerLinkRepository links) => _links = links;

        public async Task<bool> IsUserInRoleAsync(long telegramUserId, Guid serverId, UserRole requredRole, CancellationToken ct)
        {
            var link = await _links.GetAsync(telegramUserId, serverId, ct);
            return link is not null && link.Role >= requredRole;
        }
    }
}
