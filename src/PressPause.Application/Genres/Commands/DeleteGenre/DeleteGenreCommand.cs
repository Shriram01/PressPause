using MediatR;

namespace PressPause.Application.Genres.Commands.DeleteGenre;

public sealed record DeleteGenreCommand(Guid Id) : IRequest<bool>;