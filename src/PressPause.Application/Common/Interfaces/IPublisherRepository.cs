using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IPublisherRepository
{
    Task<Publisher?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Publisher> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Publisher publisher,
        CancellationToken cancellationToken = default);

    void Update(Publisher publisher);

    void Delete(Publisher publisher);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}