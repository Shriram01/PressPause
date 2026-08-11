using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Repositories;

public sealed class PublisherRepository : IPublisherRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Publishers
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
    
    public PublisherRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Publisher?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Publishers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Publisher> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Publisher> query = _context.Publishers;

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
        Publisher publisher,
        CancellationToken cancellationToken = default)
    {
        await _context.Publishers.AddAsync(publisher, cancellationToken);
    }

    public void Update(Publisher publisher)
    {
        _context.Publishers.Update(publisher);
    }

    public void Delete(Publisher publisher)
    {
        _context.Publishers.Remove(publisher);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}