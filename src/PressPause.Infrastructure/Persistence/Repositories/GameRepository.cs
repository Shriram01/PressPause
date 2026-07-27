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

    public async Task<IReadOnlyList<Game>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Games
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);
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