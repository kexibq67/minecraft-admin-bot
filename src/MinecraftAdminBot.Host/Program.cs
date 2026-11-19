using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Получаем токен из конфигурации (appsettings.json или переменные окружения)
var botToken = builder.Configuration["Telegram:Token"]
    ?? throw new InvalidOperationException("Telegram token is not configured.");

// Регистрируем ITelegramBotClient как Singleton
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
