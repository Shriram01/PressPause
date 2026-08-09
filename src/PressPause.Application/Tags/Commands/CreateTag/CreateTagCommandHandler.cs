using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Tags.Commands.CreateTag;

public sealed class CreateTagCommandHandler
    : IRequestHandler<CreateTagCommand, Guid>
{
    private readonly ITagRepository _repository;

    public CreateTagCommandHandler(
        ITagRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateTagCommand request,
        CancellationToken cancellationToken)
    {
        var tag = new Tag(
            request.Name,
            request.Slug);

        await _repository.AddAsync(
            tag,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return tag.Id;
    }
}