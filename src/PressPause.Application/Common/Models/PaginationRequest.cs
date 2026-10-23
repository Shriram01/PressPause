namespace PressPause.Application.Common.Models;

public sealed record PaginationRequest(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = null,
    bool Descending = false,
    Guid? GenreId = null,
    Guid? PlatformId = null);