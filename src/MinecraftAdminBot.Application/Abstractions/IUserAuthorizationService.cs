using MinecraftAdminBot.Domain.Authorization;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface IUserAuthorizationService
    {
        /// <summary>Проверяет имеет ли пользователь указанную роль на сервере</summary>
        Task<bool> IsUserInRoleAsync(long telegramUserId, Guid serverId, UserRole requredRole, CancellationToken ct);
    }
}
