using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Games.Commands.UpdateGame;

public sealed class UpdateGameCommandHandler
    : IRequestHandler<UpdateGameCommand>
{
    private readonly IGameRepository _repository;

    public UpdateGameCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdateGameCommand request,
        CancellationToken cancellationToken)
    {
        var game = await _repository.GetForUpdateAsync(
            request.Id,
            cancellationToken);

        if (game is null)
            throw new KeyNotFoundException("Game not found.");

        game.Update(request.Title, request.Slug, request.ReleaseDate);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}