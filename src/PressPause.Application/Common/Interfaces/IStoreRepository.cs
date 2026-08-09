using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IStoreRepository
{
    Task<Store?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Store> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Store store,
        CancellationToken cancellationToken = default);

    void Update(Store store);

    void Delete(Store store);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}