using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IGenreRepository
{
    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<Genre?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Genre> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Genre genre,
        CancellationToken cancellationToken = default);

    void Update(Genre genre);

    void Delete(Genre genre);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}