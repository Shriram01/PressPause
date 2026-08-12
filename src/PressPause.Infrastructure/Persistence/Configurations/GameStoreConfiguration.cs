using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Configurations;

public sealed class GameStoreConfiguration : IEntityTypeConfiguration<GameStore>
{
    public void Configure(EntityTypeBuilder<GameStore> builder)
    {
        builder.ToTable("GameStores");

        builder.HasKey(gameStore => new
        {
            gameStore.GameId,
            gameStore.StoreId
        });

        builder.HasOne(gameStore => gameStore.Game)
            .WithMany(game => game.GameStores)
            .HasForeignKey(gameStore => gameStore.GameId);

        builder.HasOne(gameStore => gameStore.Store)
            .WithMany(store => store.GameStores)
            .HasForeignKey(gameStore => gameStore.StoreId);
    }
}