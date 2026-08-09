using MediatR;
using PressPause.Application.Tags.Queries.GetTags;

namespace PressPause.Application.Tags.Queries.GetTagById;

public sealed record GetTagByIdQuery(Guid Id)
    : IRequest<TagDto?>;