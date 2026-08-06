using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Platforms.Commands.DeletePlatform;

public sealed class DeletePlatformCommandHandler
    : IRequestHandler<DeletePlatformCommand, bool>
{
    private readonly IPlatformRepository _repository;

    public DeletePlatformCommandHandler(
        IPlatformRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeletePlatformCommand request,
        CancellationToken cancellationToken)
    {
        var platform = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (platform is null)
        {
            return false;
        }

        _repository.Delete(platform);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}