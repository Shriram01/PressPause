using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Publishers.Commands.DeletePublisher;

public sealed class DeletePublisherCommandHandler
    : IRequestHandler<DeletePublisherCommand, bool>
{
    private readonly IPublisherRepository _repository;

    public DeletePublisherCommandHandler(
        IPublisherRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeletePublisherCommand request,
        CancellationToken cancellationToken)
    {
        var publisher = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (publisher is null)
        {
            return false;
        }

        _repository.Delete(publisher);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}