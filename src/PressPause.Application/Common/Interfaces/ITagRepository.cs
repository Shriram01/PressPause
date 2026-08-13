using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface ITagRepository
{
    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<Tag?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Tag> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Tag tag,
        CancellationToken cancellationToken = default);

    void Update(Tag tag);

    void Delete(Tag tag);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}