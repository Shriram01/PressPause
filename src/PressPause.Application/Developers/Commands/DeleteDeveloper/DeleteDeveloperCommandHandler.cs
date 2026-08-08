using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Developers.Commands.DeleteDeveloper;

public sealed class DeleteDeveloperCommandHandler
    : IRequestHandler<DeleteDeveloperCommand, bool>
{
    private readonly IDeveloperRepository _repository;

    public DeleteDeveloperCommandHandler(
        IDeveloperRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        DeleteDeveloperCommand request,
        CancellationToken cancellationToken)
    {
        var developer = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (developer is null)
        {
            return false;
        }

        _repository.Delete(developer);

        await _repository.SaveChangesAsync(cancellationToken);

        return true;
    }
}