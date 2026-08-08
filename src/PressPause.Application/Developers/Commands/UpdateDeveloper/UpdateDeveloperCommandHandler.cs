using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Developers.Commands.UpdateDeveloper;

namespace PressPause.Application.Developers.Commands.UpdateDeveloper;

public sealed class UpdateDeveloperCommandHandler
    : IRequestHandler<UpdateDeveloperCommand, bool>
{
    private readonly IDeveloperRepository _repository;

    public UpdateDeveloperCommandHandler(
        IDeveloperRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UpdateDeveloperCommand request,
        CancellationToken cancellationToken)
    {
        var developer = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (developer is null)
        {
            return false;
        }

        developer.Update(
            request.Name,
            request.Slug);

        _repository.Update(developer);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return true;
    }
}