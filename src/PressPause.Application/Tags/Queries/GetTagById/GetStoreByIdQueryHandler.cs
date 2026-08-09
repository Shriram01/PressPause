using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Tags.Queries.GetTags;

namespace PressPause.Application.Tags.Queries.GetTagById;

public sealed class GetTagByIdQueryHandler
    : IRequestHandler<GetTagByIdQuery, TagDto?>
{
    private readonly ITagRepository _repository;

    public GetTagByIdQueryHandler(
        ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<TagDto?> Handle(
        GetTagByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tag = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tag is null)
        {
            return null;
        }

        return new TagDto(
            tag.Id,
            tag.Name,
            tag.Slug);
    }
}