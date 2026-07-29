using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Repositories;

internal sealed class GameRepository : IGameRepository
{
    private readonly PressPauseDbContext _context;

    public GameRepository(PressPauseDbContext context)
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
        CancellationToken cancellationToken)
    {
        var query = _context.Games.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.Title)
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
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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