using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Repositories;

internal sealed class GameRepository : IGameRepository
{
    private readonly ApplicationDbContext _context;

    public GameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Game game, CancellationToken cancellationToken)
    {
        await _context.Games.AddAsync(game, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken)
    {
        return await _context.Games
            .AnyAsync(x => x.Slug == slug, cancellationToken);
    }

    public async Task<(IReadOnlyList<Game> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken)
    {
        IQueryable<Game> query = _context.Games
            .AsNoTracking()
            .Include(x => x.GamePlatforms)
            .ThenInclude(x => x.Platform)
            .Include(x => x.GameGenres)
            .ThenInclude(x => x.Genre)
            .Include(x => x.GameDevelopers)
            .ThenInclude(x => x.Developer)
            .Include(x => x.GamePublishers)
            .ThenInclude(x => x.Publisher);

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(g =>
                g.Title.Contains(search) ||
                g.Slug.Contains(search));
        }

        // Sorting
        query = (sortBy?.ToLower()) switch
        {
            "title" => descending
                ? query.OrderByDescending(g => g.Title)
                : query.OrderBy(g => g.Title),

            "releasedate" => descending
                ? query.OrderByDescending(g => g.ReleaseDate)
                : query.OrderBy(g => g.ReleaseDate),

            "createdat" => descending
                ? query.OrderByDescending(g => g.CreatedAt)
                : query.OrderBy(g => g.CreatedAt),

            _ => query.OrderBy(g => g.Title)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Game?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Games
            .AsNoTracking()
            .Include(x => x.GamePlatforms)
            .ThenInclude(x => x.Platform)
            .Include(x => x.GameGenres)
            .ThenInclude(x => x.Genre)
            .Include(x => x.GameDevelopers)
            .ThenInclude(x => x.Developer)
            .Include(x => x.GamePublishers)
            .ThenInclude(x => x.Publisher)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Game?> GetForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Games
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task DeleteAsync(Game game, CancellationToken cancellationToken)
    {
        _context.Games.Remove(game);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}