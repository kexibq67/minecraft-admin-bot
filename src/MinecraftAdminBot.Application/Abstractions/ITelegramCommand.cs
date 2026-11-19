using Telegram.Bot.Types;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface ITelegramCommand
    {
        /// <summary>
        /// Имя команды, например /ping
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Асинхронное выполнение команды
        /// </summary>
        Task ExecuteAsync(Update update, CancellationToken cancellationToken);
    }
}
