using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Review?> GetByUserAndGameAsync(
        string userId,
        Guid gameId,
        CancellationToken cancellationToken);

    Task<(IReadOnlyList<Review> Items, int TotalCount)> GetPagedByGameAsync(
        Guid gameId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task AddAsync(
        Review review,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Review review,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}