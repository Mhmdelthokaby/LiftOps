namespace LiftOps_BackEnd.API.Common;

public record PageRequest(int? Page, int? PageSize, string? Sort);

public record PagedResponse<T>(
    int Page,
    int PageSize,
    int TotalCount,
    string Sort,
    IReadOnlyList<T> Items
);
