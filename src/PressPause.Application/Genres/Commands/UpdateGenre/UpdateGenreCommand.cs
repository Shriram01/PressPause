using MediatR;

namespace PressPause.Application.Genres.Commands.UpdateGenre;

public sealed record UpdateGenreCommand(
    Guid Id,
    string Name,
    string Slug) : IRequest<bool>;