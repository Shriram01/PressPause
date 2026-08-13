using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Games.Commands.UpdateGame;

public sealed class UpdateGameCommandHandler
    : IRequestHandler<UpdateGameCommand>
{
    private readonly IGameRepository _repository;
    private readonly IGameRelationshipService _relationshipService;

    public UpdateGameCommandHandler(
        IGameRepository repository,
        IGameRelationshipService relationshipService)
    {
        _repository = repository;
        _relationshipService = relationshipService;
    }

    public async Task Handle(
        UpdateGameCommand request,
        CancellationToken cancellationToken)
    {
        var game = await _repository.GetForUpdateAsync(
            request.Id,
            cancellationToken);

        if (game is null)
        {
            throw new KeyNotFoundException("Game not found.");
        }

        await _relationshipService.ValidateAsync(
            request.PlatformIds,
            request.GenreIds,
            request.DeveloperIds,
            request.PublisherIds,
            request.StoreIds,
            request.TagIds,
            cancellationToken);

        game.Update(
            request.Title,
            request.Slug,
            request.ReleaseDate);

        _relationshipService.SetRelationships(
            game,
            request.PlatformIds,
            request.GenreIds,
            request.DeveloperIds,
            request.PublisherIds,
            request.StoreIds,
            request.TagIds);

        await _repository.SaveChangesAsync(
            cancellationToken);
    }
}