using PressPause.Domain.Entities;

namespace PressPause.Application.Features.UserLibrary.Queries.GetUserLibrary;

public sealed record UserGameDto(
    Guid GameId,
    string Title,
    string Slug,
    GameStatus Status,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime AddedAt);