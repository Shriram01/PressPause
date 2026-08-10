using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IPlatformRepository
{
    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<Platform?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Platform> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Platform platform,
        CancellationToken cancellationToken = default);

    void Update(Platform platform);

    void Delete(Platform platform);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}