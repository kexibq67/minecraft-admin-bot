using Microsoft.EntityFrameworkCore;
using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Application.Commands;
using MinecraftAdminBot.Application.Services;
using MinecraftAdminBot.Infrastructure.Persistence;
using MinecraftAdminBot.Infrastructure.Repositories;
using MinecraftAdminBot.Infrastructure.Telegram;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

// --- Telegram ---
var botToken = builder.Configuration["Telegram:Token"]
    ?? throw new InvalidOperationException("Telegram token is not configured.");
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

// --- Database ---
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")
                  ?? "Data Source=minecraftbot.db"));

// --- Repositories ---
builder.Services.AddScoped<IServerRepository, ServerRepository>();
builder.Services.AddScoped<IPlayerLinkRepository, PlayerLinkRepository>();
builder.Services.AddScoped<ILinkCodeRepository, LinkCodeRepository>();

// --- Application services ---
builder.Services.AddScoped<IUserAuthorizationService, UserAuthorizationService>();
builder.Services.AddScoped<IPlayerLinkingService, PlayerLinkingService>();

// --- Telegram infrastructure ---
builder.Services.AddScoped<IServerContextResolver, ChatServerContextResolver>();
builder.Services.AddScoped<CommandDispatcher>();

// --- Commands (all ITelegramCommand implementations) ---
builder.Services.AddScoped<ITelegramCommand, PingCommand>();
builder.Services.AddScoped<ITelegramCommand, LinkCommand>();
builder.Services.AddScoped<ITelegramCommand, WhoAmICommand>();

var app = builder.Build();

// --- Ensure DB created (дл€ MVP; в проде Ч migrations) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// --- Telegram webhook ---
app.MapPost("/bot/webhook", async (Update update, CommandDispatcher dispatcher, CancellationToken ct) =>
{
    await dispatcher.DispatchAsync(update, ct);
    return Results.Ok();
});

// --- Mod: link request ---
app.MapPost("/api/mod/link/request", async (
    ModLinkRequest request,
    HttpContext http,
    IServerRepository servers,
    IPlayerLinkingService linking,
    CancellationToken ct) =>
{
    var apiToken = http.Request.Headers["X-Api-Token"].FirstOrDefault();
    if (string.IsNullOrEmpty(apiToken)) return Results.Unauthorized();

    var server = await servers.GetByApiTokenAsync(apiToken, ct);
    if (server is null) return Results.Unauthorized();

    var code = await linking.GenerateLinkCodeAsync(
        server.Id, request.PlayerUuid, request.PlayerName, ct);

    return Results.Ok(new { code = code.Code, expiresAt = code.ExpiresAt, role = code.GrantedRole.ToString() });
});

var botClient = app.Services.GetRequiredService<ITelegramBotClient>();

// --- Set webhook on startup ---
var webhookUrl = builder.Configuration["Telegram:WebhookUrl"];
if (!string.IsNullOrEmpty(webhookUrl))
{
    //устанавливаем вебхук, allowedUpdates можно не указывать, если хотим получать все
    var bot = app.Services.GetRequiredService<ITelegramBotClient>();
    await botClient.SetWebhook(webhookUrl, cancellationToken: CancellationToken.None);
    Console.WriteLine($"Webhook set to: {webhookUrl}");
}

app.Run();

public record ModLinkRequest(Guid PlayerUuid, string PlayerName);