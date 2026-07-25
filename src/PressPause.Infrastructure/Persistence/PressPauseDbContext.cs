using Microsoft.EntityFrameworkCore;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence;

internal sealed class PressPauseDbContext : DbContext
{
    public DbSet<Game> Games => Set<Game>();

    public PressPauseDbContext(DbContextOptions<PressPauseDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PressPauseDbContext).Assembly);
    }
}