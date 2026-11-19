using MinecraftAdminBot.Application.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MinecraftAdminBot.Application.Commands
{
    public class PingCommand : ITelegramCommand
    {
        private readonly ITelegramBotClient _botClient;

        public PingCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public string Name => "/ping";

        public async Task ExecuteAsync(Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message) return;

            await _botClient.SendMessage(
                chatId: message.Chat.Id,
                text: "Hello, World!",
                cancellationToken: cancellationToken);
        }
    }
}
