using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Games.Queries.GetGameById;

public sealed class GetGameByIdQueryHandler
    : IRequestHandler<GetGameByIdQuery, GameDetailsDto?>
{
    private readonly IGameRepository _repository;

    public GetGameByIdQueryHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<GameDetailsDto?> Handle(
        GetGameByIdQuery request,
        CancellationToken cancellationToken)
    {
        var game = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (game is null)
            return null;

        return new GameDetailsDto(
            game.Id,
            game.Title,
            game.Slug,
            game.ReleaseDate);
    }
}