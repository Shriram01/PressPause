using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IUserGameRepository
{
    Task<UserGame?> GetByUserAndGameAsync(
        string userId,
        Guid gameId,
        CancellationToken cancellationToken);

    Task<(IReadOnlyList<UserGame> Items, int TotalCount)> GetPagedByUserAsync(
        string userId,
        int page,
        int pageSize,
        GameStatus? status,
        CancellationToken cancellationToken);

    Task AddAsync(
        UserGame userGame,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        UserGame userGame,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}