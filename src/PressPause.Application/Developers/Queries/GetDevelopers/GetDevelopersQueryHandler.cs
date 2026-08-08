using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Developers.Queries.GetDevelopers;

public sealed class GetDevelopersQueryHandler
    : IRequestHandler<GetDevelopersQuery, PagedResult<DeveloperDto>>
{
    private readonly IDeveloperRepository _repository;

    public GetDevelopersQueryHandler(
        IDeveloperRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<DeveloperDto>> Handle(
        GetDevelopersQuery request,
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
            .Select(x => new DeveloperDto(
                x.Id,
                x.Name,
                x.Slug))
            .ToList();

        return new PagedResult<DeveloperDto>
        {
            Items = dto,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}