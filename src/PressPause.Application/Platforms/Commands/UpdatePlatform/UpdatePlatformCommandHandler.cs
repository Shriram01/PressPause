using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Platforms.Commands.UpdatePlatform;

public sealed class UpdatePlatformCommandHandler
    : IRequestHandler<UpdatePlatformCommand, bool>
{
    private readonly IPlatformRepository _repository;

    public UpdatePlatformCommandHandler(
        IPlatformRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdatePlatformCommand request,
        CancellationToken cancellationToken)
    {
        var platform = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (platform is null)
        {
            return false;
        }

        platform.Update(
            request.Name,
            request.Slug);

        _repository.Update(platform);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}