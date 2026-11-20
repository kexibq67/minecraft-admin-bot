using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MinecraftAdminBot.Application.Commands
{
    [RequireRole(UserRole.Admin)]
    public class RoleCommand : ITelegramCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IRoleManagementService _roles;
        private readonly IServerContextResolver _context;

        public RoleCommand(
            ITelegramBotClient botClient,
            IRoleManagementService roles,
            IServerContextResolver context)
        {
            _botClient = botClient;
            _roles = roles;
            _context = context;
        }

        public string Name => "/role";

        public async Task ExecuteAsync(Update update, CancellationToken ct)
        {
            if (update.Message is not { } message || message.From is not { } from) return;

            var parts = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts is null || parts.Length != 3)
            {
                await _botClient.SendMessage(message.Chat.Id,
                    "Использование: `/role <username> <Viewer|Moderator|Admin>`",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }

            if (!Enum.TryParse<UserRole>(parts[2], ignoreCase: true, out var newRole))
            {
                await _botClient.SendMessage(message.Chat.Id,
                    $"Неизвестная роль `{parts[2]}`. Допустимо: Viewer, Moderator, Admin.",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }

            var serverId = await _context.ResolveServerIdAsync(update, ct);
            if (serverId is null) return; // диспетчер уже отправил сообщение об этом

            var result = await _roles.SetRoleAsync(serverId.Value, from.Id, parts[1], newRole, ct);
            await _botClient.SendMessage(message.Chat.Id, result.Message,
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}
