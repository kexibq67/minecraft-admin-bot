using MinecraftAdminBot.Application.Abstractions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Reflection;

namespace MinecraftAdminBot.Infrastructure.Telegram
{
    public class CommandDispatcher
    {
        private readonly Dictionary<string, ITelegramCommand> _commands;
        private readonly IUserAuthorizationService _auth;
        private readonly IServerContextResolver _contextResolver;
        private readonly ITelegramBotClient _botClient;

        //внедряем все зарегистрированные ITelegramCommand через DI
        public CommandDispatcher(IEnumerable<ITelegramCommand> commands, IUserAuthorizationService auth, IServerContextResolver contextResolver, ITelegramBotClient botClient)
        {
            _commands = commands.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            _auth = auth;
            _contextResolver = contextResolver;
            _botClient = botClient;
        }

        public async Task DispatchAsync(Update update, CancellationToken ct)
        {
            //только текстовые сообщения
            if (update.Type != UpdateType.Message || update.Message?.Text is not { } text)
                return;

            //извлекаем команду (первое слово, начинающееся с '/'
            var commandName = text.Split(' ').FirstOrDefault();
            if (commandName == null || !commandName.StartsWith('/')) return;
            if (!_commands.TryGetValue(commandName, out var command)) return;

            var requireRole = command.GetType().GetCustomAttribute<RequireRoleAttribute>();
            if (requireRole is not null)
            {
                if (update.Message.From?.Id is not { } userId) return;

                var serverId = await _contextResolver.ResolveServerIdAsync(update, ct);
                if (serverId is null)
                {
                    await _botClient.SendMessage(update.Message.Chat.Id,
                        "⚠️ Этот чат не привязан к серверу. Привяжите чат через панель управления.",
                        cancellationToken: ct);
                    return;
                }

                if(!await _auth.IsUserInRoleAsync(userId, serverId.Value, requireRole.Role, ct))
                {
                    await _botClient.SendMessage(update.Message.Chat.Id,
                        $"⛔ Недостаточно прав. Требуется роль: **{requireRole.Role}**.",
                        parseMode: ParseMode.Markdown,
                        cancellationToken: ct);
                    return;
                }
            }

            await command.ExecuteAsync(update, ct);

            //TODO:- else для неизвестной команды
        }
    }
}
