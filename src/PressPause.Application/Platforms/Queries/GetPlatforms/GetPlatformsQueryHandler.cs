using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Platforms.Queries.GetPlatforms;

public sealed class GetPlatformsQueryHandler
    : IRequestHandler<GetPlatformsQuery, PagedResult<PlatformDto>>
{
    private readonly IPlatformRepository _repository;

    public GetPlatformsQueryHandler(
        IPlatformRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PlatformDto>> Handle(
        GetPlatformsQuery request,
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
            .Select(x => new PlatformDto(
                x.Id,
                x.Name,
                x.Slug))
            .ToList();

        return new PagedResult<PlatformDto>
        {
            Items = dto,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}