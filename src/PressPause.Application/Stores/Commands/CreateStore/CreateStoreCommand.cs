using MediatR;

namespace PressPause.Application.Stores.Commands.CreateStore;

public sealed record CreateStoreCommand(
    string Name,
    string Slug) : IRequest<Guid>;