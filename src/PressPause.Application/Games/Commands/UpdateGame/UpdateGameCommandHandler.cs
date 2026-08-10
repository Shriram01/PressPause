using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Games.Commands.UpdateGame;

public sealed class UpdateGameCommandHandler
    : IRequestHandler<UpdateGameCommand>
{
    private readonly IGameRepository _repository;
    private readonly IPlatformRepository _platformRepository;

    public UpdateGameCommandHandler(
        IGameRepository repository,
        IPlatformRepository platformRepository)
    {
        _repository = repository;
        _platformRepository = platformRepository;
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

        foreach (var platformId in request.PlatformIds.Distinct())
        {
            bool platformExists = await _platformRepository.ExistsAsync(
                platformId,
                cancellationToken);

            if (!platformExists)
            {
                throw new KeyNotFoundException(
                    $"Platform '{platformId}' was not found.");
            }
        }

        game.Update(
            request.Title,
            request.Slug,
            request.ReleaseDate);

        game.GamePlatforms.Clear();

        foreach (var platformId in request.PlatformIds.Distinct())
        {
            game.GamePlatforms.Add(
                new GamePlatform(
                    game.Id,
                    platformId));
        }

        await _repository.SaveChangesAsync(
            cancellationToken);
    }
}