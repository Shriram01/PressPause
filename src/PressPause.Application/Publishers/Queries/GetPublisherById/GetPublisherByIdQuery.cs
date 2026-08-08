using MediatR;
using PressPause.Application.Publishers.Queries.GetPublishers;

namespace PressPause.Application.Publishers.Queries.GetPublisherById;

public sealed record GetPublisherByIdQuery(Guid Id)
    : IRequest<PublisherDto?>;