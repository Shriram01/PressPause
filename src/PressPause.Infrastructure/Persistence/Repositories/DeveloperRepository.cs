using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Repositories;

public sealed class DeveloperRepository : IDeveloperRepository
{
    private readonly ApplicationDbContext _context;

    public DeveloperRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Developer?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Developers
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Developer> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Developer> query = _context.Developers;

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
        Developer developer,
        CancellationToken cancellationToken = default)
    {
        await _context.Developers.AddAsync(developer, cancellationToken);
    }

    public void Update(Developer developer)
    {
        _context.Developers.Update(developer);
    }

    public void Delete(Developer developer)
    {
        _context.Developers.Remove(developer);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}