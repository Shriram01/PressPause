using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Games.Commands.CreateGame;

public sealed class CreateGameCommandHandler
    : IRequestHandler<CreateGameCommand, Guid>
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameRelationshipService _relationshipService;

    public CreateGameCommandHandler(
        IGameRepository gameRepository,
        IGameRelationshipService relationshipService)
    {
        _gameRepository = gameRepository;
        _relationshipService = relationshipService;
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
            throw new InvalidOperationException(
                "A game with this slug already exists.");
        }

        await _relationshipService.ValidateAsync(
            request.PlatformIds,
            request.GenreIds,
            request.DeveloperIds,
            request.PublisherIds,
            request.StoreIds,
            request.TagIds,
            cancellationToken);

        var game = new Game
        {
            Title = request.Title,
            Slug = request.Slug,
            ReleaseDate = request.ReleaseDate
        };

        _relationshipService.SetRelationships(
            game,
            request.PlatformIds,
            request.GenreIds,
            request.DeveloperIds,
            request.PublisherIds,
            request.StoreIds,
            request.TagIds);

        await _gameRepository.AddAsync(
            game,
            cancellationToken);

        await _gameRepository.SaveChangesAsync(
            cancellationToken);

        return game.Id;
    }
}