using Microsoft.EntityFrameworkCore;
using MinecraftAdminBot.Domain.Entities;

namespace MinecraftAdminBot.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Server> Servers => Set<Server>();
        public DbSet<PlayerLink> PlayerLinks => Set<PlayerLink>();
        public DbSet<LinkCode> LinkCodes => Set<LinkCode>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Server>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Name).IsUnique();
                e.HasIndex(x => x.ApiToken).IsUnique();
                e.HasIndex(x => x.TelegramChatId).IsUnique();
            });

            mb.Entity<PlayerLink>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => new { x.TelegramUserId, x.ServerId }).IsUnique();
                e.HasOne(x => x.Server).WithMany().HasForeignKey(x => x.ServerId);
            });

            mb.Entity<LinkCode>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Code).IsUnique();
            });
        }
    }
}
