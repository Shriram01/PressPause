using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Tags.Queries.GetTags;

public sealed class GetTagsQueryHandler
    : IRequestHandler<GetTagsQuery, PagedResult<TagDto>>
{
    private readonly ITagRepository _repository;

    public GetTagsQueryHandler(
        ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TagDto>> Handle(
        GetTagsQuery request,
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
            .Select(x => new TagDto(
                x.Id,
                x.Name,
                x.Slug))
            .ToList();

        return new PagedResult<TagDto>
        {
            Items = dto,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}