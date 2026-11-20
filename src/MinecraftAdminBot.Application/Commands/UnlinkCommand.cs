using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MinecraftAdminBot.Application.Commands
{
    [RequireRole(UserRole.Admin)]
    public class UnlinkCommand : ITelegramCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IRoleManagementService _roles;
        private readonly IServerContextResolver _context;

        public UnlinkCommand(
            ITelegramBotClient botClient,
            IRoleManagementService roles,
            IServerContextResolver context)
        {
            _botClient = botClient;
            _roles = roles;
            _context = context;
        }

        public string Name => "/unlink";

        public async Task ExecuteAsync(Update update, CancellationToken ct)
        {
            if (update.Message is not { } message || message.From is not { } from) return;

            var parts = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts is null || parts.Length != 2)
            {
                await _botClient.SendMessage(message.Chat.Id,
                    "Использование: `/unlink <username>`",
                    parseMode: ParseMode.Markdown, cancellationToken: ct);
                return;
            }

            var serverId = await _context.ResolveServerIdAsync(update, ct);
            if (serverId is null) return;

            var result = await _roles.UnlinkPlayerAsync(serverId.Value, from.Id, parts[1], ct);
            await _botClient.SendMessage(message.Chat.Id, result.Message,
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
    }
}
