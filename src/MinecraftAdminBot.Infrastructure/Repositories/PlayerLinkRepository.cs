using Microsoft.EntityFrameworkCore;
using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Authorization;
using MinecraftAdminBot.Domain.Entities;
using MinecraftAdminBot.Infrastructure.Persistence;

namespace MinecraftAdminBot.Infrastructure.Repositories
{
    public class PlayerLinkRepository : IPlayerLinkRepository
    {
        private readonly AppDbContext _db;
        public PlayerLinkRepository(AppDbContext db) => _db = db;

        public Task<PlayerLink?> GetAsync(long telegramUserId, Guid serverId, CancellationToken ct)
            => _db.PlayerLinks.FirstOrDefaultAsync(
                l => l.TelegramUserId == telegramUserId && l.ServerId == serverId, ct);

        public async Task AddAsync(PlayerLink link, CancellationToken ct)
        {
            _db.PlayerLinks.Add(link);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<PlayerLink>> GetByUserIdAsync(long userId, CancellationToken ct)
            => await _db.PlayerLinks.Where(l => l.TelegramUserId == userId).ToListAsync(ct);

        public Task<PlayerLink?> GetByUsernameAsync(Guid serverId, string username, CancellationToken ct)
        => _db.PlayerLinks.FirstOrDefaultAsync(
            l => l.ServerId == serverId && l.MinecraftUsername == username, ct);

        public async Task<IReadOnlyList<PlayerLink>> GetByServerIdAsync(Guid serverId, CancellationToken ct)
        => await _db.PlayerLinks.Where(l => l.ServerId == serverId).ToListAsync(ct);

        public Task<int> CountAdminsAsync(Guid serverId, CancellationToken ct)
            => _db.PlayerLinks.CountAsync(l => l.ServerId == serverId && l.Role == UserRole.Admin, ct);

        public async Task UpdateAsync(PlayerLink link, CancellationToken ct)
        {
            _db.PlayerLinks.Update(link);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(PlayerLink link, CancellationToken ct)
        {
            _db.PlayerLinks.Remove(link);
            await _db.SaveChangesAsync(ct);
        }
    }
}
