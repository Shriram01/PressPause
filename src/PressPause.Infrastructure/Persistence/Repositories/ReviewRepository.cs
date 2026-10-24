using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;
using PressPause.Infrastructure.Persistence;

namespace PressPause.Infrastructure.Persistence.Repositories;

public sealed class ReviewRepository(
    ApplicationDbContext context) : IReviewRepository
{
    public Task<Review?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return context.Set<Review>()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public Task<Review?> GetByUserAndGameAsync(
        string userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        return context.Set<Review>()
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.GameId == gameId,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Review> Items, int TotalCount)>
        GetPagedByGameAsync(
            Guid gameId,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
    {
        var query = context.Set<Review>()
            .AsNoTracking()
            .Where(x => x.GameId == gameId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task AddAsync(
        Review review,
        CancellationToken cancellationToken)
    {
        return context.Set<Review>().AddAsync(
            review,
            cancellationToken).AsTask();
    }

    public Task DeleteAsync(
        Review review,
        CancellationToken cancellationToken)
    {
        context.Set<Review>().Remove(review);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}