using MediatR;
using PressPause.Application.Authentication.Interfaces;

namespace PressPause.Application.Users;

public sealed record GetUserProfileQuery(string UserId)
    : IRequest<UserProfileDto?>;

public sealed class GetUserProfileQueryHandler
    : IRequestHandler<GetUserProfileQuery, UserProfileDto?>
{
    private readonly IUserProfileService _profileService;

    public GetUserProfileQueryHandler(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    public Task<UserProfileDto?> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        return _profileService.GetProfileAsync(
            request.UserId,
            cancellationToken);
    }
}