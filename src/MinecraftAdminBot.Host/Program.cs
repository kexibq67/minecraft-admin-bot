using Telegram.Bot;
using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Application.Commands;
using MinecraftAdminBot.Infrastructure.Telegram;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

// Получаем токен из конфигурации (appsettings.json или переменные окружения)
var botToken = builder.Configuration["Telegram:Token"]
    ?? throw new InvalidOperationException("Telegram token is not configured.");

// Регистрируем ITelegramBotClient как Singleton
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

//Регистрация команд бота
builder.Services.AddScoped<ITelegramCommand, PingCommand>();

//Регистрируем диспетчер команд
builder.Services.AddScoped<CommandDispatcher>();

var app = builder.Build();

app.MapPost("/bot/webhook", async (Update update, CommandDispatcher dispatcher, CancellationToken ct) =>
{
    await dispatcher.DispatchAsync(update, ct);
    return Results.Ok();
});

var botClient = app.Services.GetRequiredService<ITelegramBotClient>();

//url должен быть публичным и иметь https
var webhookUrl = builder.Configuration["Telegram:WebhookUrl"];
if (!string.IsNullOrEmpty(webhookUrl))
{
    //устанавливаем вебхук, allowedUpdates можно не указывать, если хотим получать все
    await botClient.SetWebhook(webhookUrl, cancellationToken: CancellationToken.None);
    Console.WriteLine($"Webhook set to: {webhookUrl}");
}

app.Run();
