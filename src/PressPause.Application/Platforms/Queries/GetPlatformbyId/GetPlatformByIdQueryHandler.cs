using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Platforms.Queries.GetPlatforms;

namespace PressPause.Application.Platforms.Queries.GetPlatformById;

public sealed class GetPlatformByIdQueryHandler
    : IRequestHandler<GetPlatformByIdQuery, PlatformDto?>
{
    private readonly IPlatformRepository _repository;

    public GetPlatformByIdQueryHandler(
        IPlatformRepository repository)
    {
        _repository = repository;
    }

    public async Task<PlatformDto?> Handle(
        GetPlatformByIdQuery request,
        CancellationToken cancellationToken)
    {
        var platform = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (platform is null)
        {
            return null;
        }

        return new PlatformDto(
            platform.Id,
            platform.Name,
            platform.Slug);
    }
}