using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IGameRepository
{
    Task AddAsync(Game game, CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken);

    Task<(IReadOnlyList<Game> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken);

    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Game?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken);

    Task DeleteAsync(Game game, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}