using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Publishers.Commands.CreatePublisher;

public sealed class CreatePublisherCommandHandler
    : IRequestHandler<CreatePublisherCommand, Guid>
{
    private readonly IPublisherRepository _repository;

    public CreatePublisherCommandHandler(
        IPublisherRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreatePublisherCommand request,
        CancellationToken cancellationToken)
    {
        var publisher = new Publisher(
            request.Name,
            request.Slug);

        await _repository.AddAsync(
            publisher,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return publisher.Id;
    }
}