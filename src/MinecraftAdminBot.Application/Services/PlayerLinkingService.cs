using System.Security.Cryptography;
using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;
using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Services
{
    public class PlayerLinkingService : IPlayerLinkingService
    {
        private const int CodeLength = 6;
        private static readonly TimeSpan CodeLifeTime = TimeSpan.FromMinutes(10);

        private readonly ILinkCodeRepository _codes;
        private readonly IPlayerLinkRepository _links;
        private readonly IServerRepository _servers;

        public PlayerLinkingService(ILinkCodeRepository codes, IPlayerLinkRepository links, IServerRepository servers)
        {
            _codes = codes;
            _links = links;
            _servers = servers;
        }

        public async Task<LinkCode> GenerateLinkCodeAsync(Guid serverId, Guid playerUuid, string playerName, CancellationToken ct)
        {
            var hasAdmin = await _servers.HasAnyAdminAsync(serverId, ct);
            var grantedRole = hasAdmin ? UserRole.Viewer : UserRole.Admin;

            var code = new LinkCode
            {
                Id = Guid.NewGuid(),
                Code = GenerateCode(),
                ServerId = serverId,
                MinecraftUuid = playerUuid,
                MinecraftUsername = playerName,
                GrantedRole = grantedRole,
                ExpiresAt = DateTime.UtcNow.Add(CodeLifeTime),
                IsUsed = false
            };

            await _codes.AddAsync(code, ct);
            return code;
        }

        public async Task<LinkResult> CompleteLinkAsync(long telegramUserId, string code, CancellationToken ct)
        {
            var linkCode = await _codes.GetActiveByCodeAsync(code, ct);

            if (linkCode is null)
                return new(false, "Код не найден или уже использован.", null, null);

            if (linkCode.ExpiresAt < DateTime.UtcNow)
                return new(false, "Срок действия кода истёк. Запросите новый через /tg link на сервере.", null, null);

            //помечаем код как использованный
            linkCode.IsUsed = true;
            await _codes.UpdateAsync(linkCode, ct);

            //создаём или обновляем привязку
            var existing = await _links.GetAsync(telegramUserId, linkCode.ServerId, ct);

            if (existing is not null)
                return new(false, "Вы уже привязаны к этому серверу.", null, null);

            var link = new PlayerLink
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                MinecraftUuid = linkCode.MinecraftUuid,
                MinecraftUsername = linkCode.MinecraftUsername,
                ServerId = linkCode.ServerId,
                Role = linkCode.GrantedRole,
                LinkedAt = DateTime.UtcNow
            };
            await _links.AddAsync(link, ct);

            return new (true, $"✅ Вы привязаны как **{linkCode.MinecraftUsername}** с ролью **{linkCode.GrantedRole}**.", linkCode.ServerId, linkCode.GrantedRole);
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // без похожих символов
            Span<char> buffer = stackalloc char[CodeLength];
            for (var i = 0; i < CodeLength; i++)
                buffer[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            return new string(buffer);
        }
    }
}
