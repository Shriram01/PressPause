using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Tags.Commands.UpdateTag;

namespace PressPause.Application.Tags.Commands.UpdateTag;

public sealed class UpdateTagCommandHandler
    : IRequestHandler<UpdateTagCommand, bool>
{
    private readonly ITagRepository _repository;

    public UpdateTagCommandHandler(
        ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateTagCommand request,
        CancellationToken cancellationToken)
    {
        var tag = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (tag is null)
        {
            return false;
        }

        tag.Update(
            request.Name,
            request.Slug);

        _repository.Update(tag);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}