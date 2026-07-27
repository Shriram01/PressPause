using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Games.Commands.DeleteGame;

public sealed class DeleteGameCommandHandler
    : IRequestHandler<DeleteGameCommand>
{
    private readonly IGameRepository _repository;

    public DeleteGameCommandHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        DeleteGameCommand request,
        CancellationToken cancellationToken)
    {
        var game = await _repository.GetForUpdateAsync(
            request.Id,
            cancellationToken);

        if (game is null)
            throw new KeyNotFoundException("Game not found.");

        await _repository.DeleteAsync(game, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}