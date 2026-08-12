using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Repositories;

public sealed class StoreRepository : IStoreRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
    
    public StoreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Store?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Stores
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Store> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Store> query = _context.Stores;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Slug.Contains(search));
        }

        query = (sortBy?.ToLower()) switch
        {
            "slug" => descending
                ? query.OrderByDescending(x => x.Slug)
                : query.OrderBy(x => x.Slug),

            _ => descending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(
        Store store,
        CancellationToken cancellationToken = default)
    {
        await _context.Stores.AddAsync(store, cancellationToken);
    }

    public void Update(Store store)
    {
        _context.Stores.Update(store);
    }

    public void Delete(Store store)
    {
        _context.Stores.Remove(store);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}