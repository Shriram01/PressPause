using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Publishers.Queries.GetPublishers;

namespace PressPause.Application.Publishers.Queries.GetPublisherById;

public sealed class GetPublisherByIdQueryHandler
    : IRequestHandler<GetPublisherByIdQuery, PublisherDto?>
{
    private readonly IPublisherRepository _repository;

    public GetPublisherByIdQueryHandler(
        IPublisherRepository repository)
    {
        _repository = repository;
    }

    public async Task<PublisherDto?> Handle(
        GetPublisherByIdQuery request,
        CancellationToken cancellationToken)
    {
        var publisher = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (publisher is null)
        {
            return null;
        }

        return new PublisherDto(
            publisher.Id,
            publisher.Name,
            publisher.Slug);
    }
}