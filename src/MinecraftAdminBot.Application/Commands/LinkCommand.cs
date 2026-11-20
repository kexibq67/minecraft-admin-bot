using MinecraftAdminBot.Application.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MinecraftAdminBot.Application.Commands
{
    public class LinkCommand : ITelegramCommand
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IPlayerLinkingService _linking;

        public LinkCommand(ITelegramBotClient botClient, IPlayerLinkingService linking)
        {
            _botClient = botClient;
            _linking = linking;
        }

        public string Name => "/link";

        public async Task ExecuteAsync(Update update, CancellationToken ct)
        {
            if (update.Message is not { } message || message.From is not { } from) return;

            var parts = message.Text?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts is null || parts.Length < 2)
            {
                await _botClient.SendMessage(message.Chat.Id,
                    "Использование: `/link <код>`. Получите код на сервере командой `/tg link`.",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: ct);
                return;
            }

            var result = await _linking.CompleteLinkAsync(from.Id, parts[1].Trim(), ct);

            await _botClient.SendMessage(message.Chat.Id, result.Message,
                parseMode: ParseMode.Markdown,
                cancellationToken: ct);
        }
    }
}
