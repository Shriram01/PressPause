using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Games.Commands.CreateGame;

public sealed class CreateGameCommandHandler
    : IRequestHandler<CreateGameCommand, Guid>
{
    private readonly IGameRepository _gameRepository;

    public CreateGameCommandHandler(IGameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public async Task<Guid> Handle(
        CreateGameCommand request,
        CancellationToken cancellationToken)
    {
        bool slugExists = await _gameRepository.SlugExistsAsync(
            request.Slug,
            cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException("A game with this slug already exists.");
        }

        var game = new Game
        {
            Title = request.Title,
            Slug = request.Slug,
            ReleaseDate = request.ReleaseDate
        };

        await _gameRepository.AddAsync(game, cancellationToken);
        await _gameRepository.SaveChangesAsync(cancellationToken);

        return game.Id;
    }
}