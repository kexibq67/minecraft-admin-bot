using MinecraftAdminBot.Application.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MinecraftAdminBot.Application.Commands
{
    public class WhoAmICommand : ITelegramCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IPlayerLinkRepository _links;
        private readonly IServerRepository _servers;

        public WhoAmICommand(ITelegramBotClient botClient, IPlayerLinkRepository links, IServerRepository servers)
        {
            _botClient = botClient;
            _links = links;
            _servers = servers;
        }

        public string Name => "/whoami";

        public async Task ExecuteAsync(Update update, CancellationToken ct)
        {
            if (update.Message is not { } message || message.From is not { } from) return;

            var links = await _links.GetByUserIdAsync(from.Id, ct);
            if(links.Count == 0)
            {
                await _botClient.SendMessage(message.Chat.Id,
                    "Вы пока не привязаны ни к одному серверу. Используйте `/tg link` на сервере.",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
                return;
            }

            var lines = new List<string> { $"Ваш Telegram ID: `{from.Id}`\n" };
            foreach (var link in links)
            {
                var server = await _servers.GetByIdAsync(link.ServerId, ct);
                lines.Add($"• **{server?.Name ?? link.ServerId.ToString()}** — `{link.MinecraftUsername}` ({link.Role})");

                await _botClient.SendMessage(message.Chat.Id, string.Join("\n", lines),
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
            }
        }
    }
}
