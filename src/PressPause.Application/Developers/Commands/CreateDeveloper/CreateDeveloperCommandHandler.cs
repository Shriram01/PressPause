using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Developers.Commands.CreateDeveloper;

public sealed class CreateDeveloperCommandHandler
    : IRequestHandler<CreateDeveloperCommand, Guid>
{
    private readonly IDeveloperRepository _repository;

    public CreateDeveloperCommandHandler(
        IDeveloperRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreateDeveloperCommand request,
        CancellationToken cancellationToken)
    {
        var developer = new Developer(
            request.Name,
            request.Slug);

        await _repository.AddAsync(
            developer,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return developer.Id;
    }
}