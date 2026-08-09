using MediatR;

namespace PressPause.Application.Stores.Commands.DeleteStore;

public sealed record DeleteStoreCommand(Guid Id) : IRequest<bool>;