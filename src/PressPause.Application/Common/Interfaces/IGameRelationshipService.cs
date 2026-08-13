using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Interfaces;

public interface IGameRelationshipService
{
    Task ValidateAsync(
        IReadOnlyCollection<Guid> platformIds,
        IReadOnlyCollection<Guid> genreIds,
        IReadOnlyCollection<Guid> developerIds,
        IReadOnlyCollection<Guid> publisherIds,
        IReadOnlyCollection<Guid> storeIds,
        IReadOnlyCollection<Guid> tagIds,
        CancellationToken cancellationToken = default);

    void SetRelationships(
        Game game,
        IReadOnlyCollection<Guid> platformIds,
        IReadOnlyCollection<Guid> genreIds,
        IReadOnlyCollection<Guid> developerIds,
        IReadOnlyCollection<Guid> publisherIds,
        IReadOnlyCollection<Guid> storeIds,
        IReadOnlyCollection<Guid> tagIds);
}