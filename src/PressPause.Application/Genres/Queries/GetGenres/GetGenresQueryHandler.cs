using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Genres.Queries.GetGenres;

public sealed class GetGenresQueryHandler
    : IRequestHandler<GetGenresQuery, PagedResult<GenreDto>>
{
    private readonly IGenreRepository _repository;

    public GetGenresQueryHandler(
        IGenreRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GenreDto>> Handle(
        GetGenresQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.SortBy,
            request.Descending,
            cancellationToken);

        var dto = items
            .Select(x => new GenreDto(
                x.Id,
                x.Name,
                x.Slug))
            .ToList();

        return new PagedResult<GenreDto>
        {
            Items = dto,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}