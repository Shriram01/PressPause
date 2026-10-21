using Microsoft.EntityFrameworkCore;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Infrastructure.Persistence.Repositories;

internal sealed class UserGameRepository : IUserGameRepository
{
    private readonly ApplicationDbContext _context;

    public UserGameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserGame?> GetByUserAndGameAsync(
        string userId,
        Guid gameId,
        CancellationToken cancellationToken)
    {
        return await _context.UserGames
            .Include(x => x.Game)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                x => x.UserId == userId && x.GameId == gameId,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<UserGame> Items, int TotalCount)>
        GetPagedByUserAsync(
            string userId,
            int page,
            int pageSize,
            GameStatus? status,
            CancellationToken cancellationToken)
    {
        IQueryable<UserGame> query = _context.UserGames
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Game);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .ThenBy(x => x.Game.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(
        UserGame userGame,
        CancellationToken cancellationToken)
    {
        await _context.UserGames.AddAsync(userGame, cancellationToken);
    }

    public Task DeleteAsync(
        UserGame userGame,
        CancellationToken cancellationToken)
    {
        _context.UserGames.Remove(userGame);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}