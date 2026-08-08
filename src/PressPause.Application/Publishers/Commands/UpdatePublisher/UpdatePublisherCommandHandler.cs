using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Publishers.Commands.UpdatePublisher;

namespace PressPause.Application.Publishers.Commands.UpdatePublisher;

public sealed class UpdatePublisherCommandHandler
    : IRequestHandler<UpdatePublisherCommand, bool>
{
    private readonly IPublisherRepository _repository;

    public UpdatePublisherCommandHandler(
        IPublisherRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdatePublisherCommand request,
        CancellationToken cancellationToken)
    {
        var publisher = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (publisher is null)
        {
            return false;
        }

        publisher.Update(
            request.Name,
            request.Slug);

        _repository.Update(publisher);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}