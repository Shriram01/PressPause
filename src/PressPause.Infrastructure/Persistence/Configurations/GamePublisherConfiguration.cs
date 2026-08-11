using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Configurations;

public sealed class GamePublisherConfiguration
    : IEntityTypeConfiguration<GamePublisher>
{
    public void Configure(
        EntityTypeBuilder<GamePublisher> builder)
    {
        builder.ToTable("GamePublishers");

        builder.HasKey(x => new
        {
            x.GameId,
            x.PublisherId
        });

        builder
            .HasOne(x => x.Game)
            .WithMany(x => x.GamePublishers)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Publisher)
            .WithMany(x => x.GamePublishers)
            .HasForeignKey(x => x.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}