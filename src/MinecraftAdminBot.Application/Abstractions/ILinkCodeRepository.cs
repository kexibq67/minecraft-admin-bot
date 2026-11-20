using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Application.Abstractions
{
    public interface ILinkCodeRepository
    {
        Task<LinkCode?> GetActiveByCodeAsync(string code, CancellationToken ct);
        Task AddAsync(LinkCode code, CancellationToken ct);
        Task UpdateAsync(LinkCode code, CancellationToken ct);
    }
}
