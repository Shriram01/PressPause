namespace PressPause.Application.Common.Models;

public sealed class PaginationRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}