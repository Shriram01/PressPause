using MediatR;

namespace PressPause.Application.Genres.Commands.CreateGenre;

public sealed record CreateGenreCommand(
    string Name,
    string Slug) : IRequest<Guid>;