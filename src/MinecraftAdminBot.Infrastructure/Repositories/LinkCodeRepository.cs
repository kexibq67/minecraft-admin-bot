using Microsoft.EntityFrameworkCore;
using MinecraftAdminBot.Application.Abstractions;
using MinecraftAdminBot.Domain.Entities;
using MinecraftAdminBot.Infrastructure.Persistence;

namespace MinecraftAdminBot.Infrastructure.Repositories
{
    public class LinkCodeRepository : ILinkCodeRepository
    {
        private readonly AppDbContext _db;
        public LinkCodeRepository(AppDbContext db) => _db = db;

        public Task<LinkCode?> GetActiveByCodeAsync(string code, CancellationToken ct)
            => _db.LinkCodes.FirstOrDefaultAsync(c => c.Code == code && !c.IsUsed, ct);

        public async Task AddAsync(LinkCode code, CancellationToken ct)
        {
            _db.LinkCodes.Add(code);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(LinkCode code, CancellationToken ct)
        {
            _db.LinkCodes.Update(code);
            await _db.SaveChangesAsync(ct);
        }
    }
}
