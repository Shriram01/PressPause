using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Tags.Commands.DeleteTag;

public sealed class DeleteTagCommandHandler
    : IRequestHandler<DeleteTagCommand, bool>
{
    private readonly ITagRepository _repository;

    public DeleteTagCommandHandler(
        ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteTagCommand request,
        CancellationToken cancellationToken)
    {
        var tag = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tag is null)
        {
            return false;
        }

        _repository.Delete(tag);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}