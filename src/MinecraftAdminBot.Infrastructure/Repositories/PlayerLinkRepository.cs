using Microsoft.EntityFrameworkCore;
using MinecraftAdminBot.Application.Abstractions;
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
    }
}
