using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Platforms.Commands.CreatePlatform;

public sealed class CreatePlatformCommandHandler
    : IRequestHandler<CreatePlatformCommand, Guid>
{
    private readonly IPlatformRepository _repository;

    public CreatePlatformCommandHandler(
        IPlatformRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(
        CreatePlatformCommand request,
        CancellationToken cancellationToken)
    {
        var platform = new Platform(
            request.Name,
            request.Slug);

        await _repository.AddAsync(
            platform,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return platform.Id;
    }
}