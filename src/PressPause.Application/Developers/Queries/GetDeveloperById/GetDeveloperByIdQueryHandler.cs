using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Developers.Queries.GetDevelopers;

namespace PressPause.Application.Developers.Queries.GetDeveloperById;

public sealed class GetDeveloperByIdQueryHandler
    : IRequestHandler<GetDeveloperByIdQuery, DeveloperDto?>
{
    private readonly IDeveloperRepository _repository;

    public GetDeveloperByIdQueryHandler(
        IDeveloperRepository repository)
    {
        _repository = repository;
    }

    public async Task<DeveloperDto?> Handle(
        GetDeveloperByIdQuery request,
        CancellationToken cancellationToken)
    {
        var developer = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (developer is null)
        {
            return null;
        }

        return new DeveloperDto(
            developer.Id,
            developer.Name,
            developer.Slug);
    }
}