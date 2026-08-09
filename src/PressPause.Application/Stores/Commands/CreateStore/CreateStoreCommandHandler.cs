using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Stores.Commands.CreateStore;

public sealed class CreateStoreCommandHandler
    : IRequestHandler<CreateStoreCommand, Guid>
{
    private readonly IStoreRepository _repository;

    public CreateStoreCommandHandler(
        IStoreRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateStoreCommand request,
        CancellationToken cancellationToken)
    {
        var store = new Store(
            request.Name,
            request.Slug);

        await _repository.AddAsync(
            store,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return store.Id;
    }
}