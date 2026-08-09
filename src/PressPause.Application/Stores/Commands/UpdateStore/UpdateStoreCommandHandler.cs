using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Stores.Commands.UpdateStore;

namespace PressPause.Application.Stores.Commands.UpdateStore;

public sealed class UpdateStoreCommandHandler
    : IRequestHandler<UpdateStoreCommand, bool>
{
    private readonly IStoreRepository _repository;

    public UpdateStoreCommandHandler(
        IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateStoreCommand request,
        CancellationToken cancellationToken)
    {
        var store = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (store is null)
        {
            return false;
        }

        store.Update(
            request.Name,
            request.Slug);

        _repository.Update(store);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}