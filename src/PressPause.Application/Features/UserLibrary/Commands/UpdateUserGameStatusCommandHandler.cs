using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Features.UserLibrary.Commands;

public sealed class UpdateUserGameStatusCommandHandler
    : IRequestHandler<UpdateUserGameStatusCommand>
{
    private readonly IUserGameRepository _repository;

    public UpdateUserGameStatusCommandHandler(
        IUserGameRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(
        UpdateUserGameStatusCommand request,
        CancellationToken cancellationToken)
    {
        var userGame = await _repository.GetByUserAndGameAsync(
            request.UserId,
            request.GameId,
            cancellationToken);

        if (userGame is null)
        {
            throw new KeyNotFoundException(
                "Game not found in the user's library.");
        }

        userGame.UpdateStatus(request.Status);

        await _repository.SaveChangesAsync(cancellationToken);
    }
}