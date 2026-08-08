using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Publishers.Queries.GetPublishers;

public sealed class GetPublishersQueryHandler
    : IRequestHandler<GetPublishersQuery, PagedResult<PublisherDto>>
{
    private readonly IPublisherRepository _repository;

    public GetPublishersQueryHandler(
        IPublisherRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PublisherDto>> Handle(
        GetPublishersQuery request,
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
            .Select(x => new PublisherDto(
                x.Id,
                x.Name,
                x.Slug))
            .ToList();

        return new PagedResult<PublisherDto>
        {
            Items = dto,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}