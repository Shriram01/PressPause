using MediatR;

namespace PressPause.Application.Stores.Commands.UpdateStore;

public sealed record UpdateStoreCommand(
    Guid Id,
    string Name,
    string Slug) : IRequest<bool>;