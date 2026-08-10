using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Configurations;

public sealed class GamePlatformConfiguration
    : IEntityTypeConfiguration<GamePlatform>
{
    public void Configure(
        EntityTypeBuilder<GamePlatform> builder)
    {
        builder.ToTable("GamePlatforms");

        builder.HasKey(x => new
        {
            x.GameId,
            x.PlatformId
        });

        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.GamePlatforms)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Platform)
            .WithMany(x => x.GamePlatforms)
            .HasForeignKey(x => x.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}