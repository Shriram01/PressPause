using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IDeveloperRepository
{
    Task<Developer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Developer> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Developer developer,
        CancellationToken cancellationToken = default);

    void Update(Developer developer);

    void Delete(Developer developer);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}