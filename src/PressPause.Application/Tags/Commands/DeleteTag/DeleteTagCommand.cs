using MediatR;

namespace PressPause.Application.Tags.Commands.DeleteTag;

public sealed record DeleteTagCommand(Guid Id) : IRequest<bool>;