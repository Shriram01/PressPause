using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Configurations;

public sealed class GameTagConfiguration
    : IEntityTypeConfiguration<GameTag>
{
    public void Configure(
        EntityTypeBuilder<GameTag> builder)
    {
        builder.ToTable("GameTags");

        builder.HasKey(x => new
        {
            x.GameId,
            x.TagId
        });

        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.GameTags)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Tag)
            .WithMany(x => x.GameTags)
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}