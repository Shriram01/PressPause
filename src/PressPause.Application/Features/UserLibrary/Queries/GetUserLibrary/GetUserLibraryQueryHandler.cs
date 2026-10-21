using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Features.UserLibrary.Queries.GetUserLibrary;

public sealed class GetUserLibraryQueryHandler
    : IRequestHandler<GetUserLibraryQuery, PagedResult<UserGameDto>>
{
    private readonly IUserGameRepository _repository;

    public GetUserLibraryQueryHandler(IUserGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<UserGameDto>> Handle(
        GetUserLibraryQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetPagedByUserAsync(
            request.UserId,
            request.Page,
            request.PageSize,
            request.Status,
            cancellationToken);

        return new PagedResult<UserGameDto>
        {
            Items = items.Select(x => new UserGameDto(
                x.GameId,
                x.Game.Title,
                x.Game.Slug,
                x.Status,
                x.StartedAt,
                x.CompletedAt,
                x.CreatedAt
            )).ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}