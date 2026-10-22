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
        Guid? genreId,
        Guid? platformId,
        CancellationToken cancellationToken)
    {
        IQueryable<Game> query = _context.Games
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.GamePlatforms)
            .ThenInclude(x => x.Platform)
            .Include(x => x.GameGenres)
            .ThenInclude(x => x.Genre)
            .Include(x => x.GameDevelopers)
            .ThenInclude(x => x.Developer)
            .Include(x => x.GamePublishers)
            .ThenInclude(x => x.Publisher)
            .Include(x => x.GameStores)
            .ThenInclude(x => x.Store)
            .Include(x => x.GameTags)
            .ThenInclude(x => x.Tag);

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(g =>
                g.Title.Contains(search) ||
                g.Slug.Contains(search));
        }

        // Filters
        if (genreId.HasValue)
        {
            query = query.Where(g =>
                g.GameGenres.Any(gg => gg.GenreId == genreId.Value));
        }

        if (platformId.HasValue)
        {
            query = query.Where(g =>
                g.GamePlatforms.Any(gp => gp.PlatformId == platformId.Value));
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
            .AsSplitQuery()
            .Include(x => x.GamePlatforms)
            .ThenInclude(x => x.Platform)
            .Include(x => x.GameGenres)
            .ThenInclude(x => x.Genre)
            .Include(x => x.GameDevelopers)
            .ThenInclude(x => x.Developer)
            .Include(x => x.GamePublishers)
            .ThenInclude(x => x.Publisher)
            .Include(x => x.GameStores)
            .ThenInclude(x => x.Store)
            .Include(x => x.GameTags)
            .ThenInclude(x => x.Tag)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<Game?> GetForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Games
            .Include(x => x.GamePlatforms)
            .Include(x => x.GameGenres)
            .Include(x => x.GameDevelopers)
            .Include(x => x.GamePublishers)
            .Include(x => x.GameStores)
            .Include(x => x.GameTags)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
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