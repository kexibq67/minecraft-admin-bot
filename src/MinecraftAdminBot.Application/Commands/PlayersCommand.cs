using System.Text;
using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MinecraftAdminBot.Application.Commands
{
    [RequireRole(UserRole.Viewer)]
    public class PlayersCommand : ITelegramCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IRoleManagementService _roles;
        private readonly IServerContextResolver _context;

        public PlayersCommand(
            ITelegramBotClient botClient,
            IRoleManagementService roles,
            IServerContextResolver context)
        {
            _botClient = botClient;
            _roles = roles;
            _context = context;
        }

        public string Name => "/players";

        public async Task ExecuteAsync(Update update, CancellationToken ct)
        {
            if (update.Message is not { } message) return;

            var serverId = await _context.ResolveServerIdAsync(update, ct);
            if (serverId is null) return;

            var players = await _roles.GetPlayersAsync(serverId.Value, ct);
            if (players.Count == 0)
            {
                await _botClient.SendMessage(message.Chat.Id,
                    "На этом сервере пока нет привязанных игроков.",
                    cancellationToken: ct);
                return;
            }

            var sb = new StringBuilder("👥 **Привязанные игроки:**\n\n");
            foreach (var link in players.OrderByDescending(l => l.Role).ThenBy(l => l.MinecraftUsername))
            {
                var icon = link.Role switch
                {
                    UserRole.Admin => "👑",
                    UserRole.Moderator => "🛡",
                    _ => "👤"
                };
                sb.AppendLine($"{icon} `{link.MinecraftUsername}` — {link.Role}");
            }

            await _botClient.SendMessage(message.Chat.Id, sb.ToString(),
                parseMode: ParseMode.Markdown, cancellationToken: ct);
        }
}
