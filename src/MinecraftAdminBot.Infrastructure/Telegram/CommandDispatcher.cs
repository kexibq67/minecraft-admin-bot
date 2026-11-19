using MinecraftAdminBot.Application.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace MinecraftAdminBot.Infrastructure.Telegram
{
    public class CommandDispatcher
    {
        private readonly Dictionary<string, ITelegramCommand> _commands;

        //внедряем все зарегистрированные ITelegramCommand через DI
        public CommandDispatcher(IEnumerable<ITelegramCommand> commands)
        {
            _commands = commands.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
        }

        public async Task DispatchAsync(Update update, CancellationToken cancellationToken)
        {
            //только текстовые сообщения
            if (update.Type != UpdateType.Message || update.Message?.Text is not { } text)
                return;

            //извлекаем команду (первое слово, начинающееся с '/'
            var commandName = text.Split(' ').FirstOrDefault();
            if (commandName == null || !commandName.StartsWith('/'))
                return;

            if (_commands.TryGetValue(commandName, out var command))
            {
                await command.ExecuteAsync(update, cancellationToken);
            }

            //TODO:- else для неизвестной команды
        }
    }
}
