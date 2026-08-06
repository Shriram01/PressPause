using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Repositories;

public sealed class PlatformRepository : IPlatformRepository
{
    private readonly ApplicationDbContext _context;

    public PlatformRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Platform?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Platforms
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<(IReadOnlyList<Platform> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Platform> query = _context.Platforms;

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
        Platform platform,
        CancellationToken cancellationToken = default)
    {
        await _context.Platforms.AddAsync(platform, cancellationToken);
    }

    public void Update(Platform platform)
    {
        _context.Platforms.Update(platform);
    }

    public void Delete(Platform platform)
    {
        _context.Platforms.Remove(platform);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}