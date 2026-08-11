using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Configurations;

public sealed class GameDeveloperConfiguration
    : IEntityTypeConfiguration<GameDeveloper>
{
    public void Configure(
        EntityTypeBuilder<GameDeveloper> builder)
    {
        builder.ToTable("GameDevelopers");

        builder.HasKey(x => new
        {
            x.GameId,
            x.DeveloperId
        });

        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.GameDevelopers)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Developer)
            .WithMany(x => x.GameDevelopers)
            .HasForeignKey(x => x.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}