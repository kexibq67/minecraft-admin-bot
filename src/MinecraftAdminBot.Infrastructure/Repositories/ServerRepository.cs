using Microsoft.EntityFrameworkCore;
using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;
using MinecraftAdminBot.Domain.Entities;
using MinecraftAdminBot.Infrastructure.Persistence;

namespace MinecraftAdminBot.Infrastructure.Repositories
{
    public class ServerRepository : IServerRepository
    {
        private readonly AppDbContext _db;
        public ServerRepository(AppDbContext db) => _db = db;

        public Task<Server?> GetByApiTokenAsync(string apiToken, CancellationToken ct)
            => _db.Servers.FirstOrDefaultAsync(s => s.ApiToken == apiToken, ct);

        public Task<Server?> GetByChatIdAsync(long chatId, CancellationToken ct)
            => _db.Servers.FirstOrDefaultAsync(s => s.TelegramChatId == chatId, ct);

        public Task<Server?> GetByIdAsync(Guid id, CancellationToken ct)
            => _db.Servers.FirstOrDefaultAsync(s => s.Id == id, ct);

        public Task<bool> HasAnyAdminAsync(Guid serverId, CancellationToken ct)
            => _db.PlayerLinks.AnyAsync(l => l.ServerId == serverId && l.Role == UserRole.Admin, ct);
    }
}
