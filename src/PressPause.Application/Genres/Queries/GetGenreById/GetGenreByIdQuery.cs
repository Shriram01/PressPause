using MediatR;
using PressPause.Application.Genres.Queries.GetGenres;

namespace PressPause.Application.Genres.Queries.GetGenreById;

public sealed record GetGenreByIdQuery(Guid Id)
    : IRequest<GenreDto?>;