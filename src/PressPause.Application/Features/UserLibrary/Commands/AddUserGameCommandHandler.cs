using MediatR;
using PressPause.Application.Common.Exceptions;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Features.UserLibrary.Commands;

public sealed class AddUserGameCommandHandler
    : IRequestHandler<AddUserGameCommand, Guid>
{
    private readonly IUserGameRepository _userGameRepository;
    private readonly IGameRepository _gameRepository;

    public AddUserGameCommandHandler(
        IUserGameRepository userGameRepository,
        IGameRepository gameRepository)
    {
        _userGameRepository = userGameRepository;
        _gameRepository = gameRepository;
    }

    public async Task<Guid> Handle(
        AddUserGameCommand request,
        CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(
            request.GameId,
            cancellationToken);

        if (game is null)
        {
            throw new KeyNotFoundException("Game not found.");
        }

        var existingUserGame = await _userGameRepository.GetByUserAndGameAsync(
            request.UserId,
            request.GameId,
            cancellationToken);

        if (existingUserGame is not null)
        {
            throw new ConflictException(
                "This game is already in the user's library.");
        }

        var userGame = new UserGame
        {
            UserId = request.UserId,
            GameId = request.GameId
        };

        await _userGameRepository.AddAsync(userGame, cancellationToken);
        await _userGameRepository.SaveChangesAsync(cancellationToken);

        return userGame.Id;
    }
}