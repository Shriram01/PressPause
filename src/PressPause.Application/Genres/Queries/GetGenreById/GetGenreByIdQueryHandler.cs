using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Genres.Queries.GetGenres;

namespace PressPause.Application.Genres.Queries.GetGenreById;

public sealed class GetGenreByIdQueryHandler
    : IRequestHandler<GetGenreByIdQuery, GenreDto?>
{
    private readonly IGenreRepository _repository;

    public GetGenreByIdQueryHandler(
        IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<GenreDto?> Handle(
        GetGenreByIdQuery request,
        CancellationToken cancellationToken)
    {
        var genre = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (genre is null)
        {
            return null;
        }

        return new GenreDto(
            genre.Id,
            genre.Name,
            genre.Slug);
    }
}