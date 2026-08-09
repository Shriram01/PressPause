using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Stores.Commands.DeleteStore;

public sealed class DeleteStoreCommandHandler
    : IRequestHandler<DeleteStoreCommand, bool>
{
    private readonly IStoreRepository _repository;

    public DeleteStoreCommandHandler(
        IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteStoreCommand request,
        CancellationToken cancellationToken)
    {
        var store = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (store is null)
        {
            return false;
        }

        _repository.Delete(store);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}