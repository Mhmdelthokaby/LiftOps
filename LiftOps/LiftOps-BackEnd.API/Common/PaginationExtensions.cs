namespace LiftOps_BackEnd.API.Common;

public static class PaginationExtensions
{
    public static bool TryNormalize(PageRequest request, out int page, out int pageSize, out string sort, out string? error)
    {
        error = null;
        page = request.Page ?? 0;
        pageSize = request.PageSize ?? 0;
        sort = request.Sort?.Trim() ?? string.Empty;

        if (request.Page is null || request.PageSize is null || string.IsNullOrWhiteSpace(request.Sort))
        {
            error = "Query parameters page, pageSize, and sort are required.";
            return false;
        }

        if (page <= 0 || pageSize <= 0)
        {
            error = "page and pageSize must be greater than zero.";
            return false;
        }

        if (pageSize > 200)
        {
            error = "pageSize must be less than or equal to 200.";
            return false;
        }

        return true;
    }

    public static IReadOnlyList<T> ApplyPaging<T>(this IEnumerable<T> source, int page, int pageSize)
    {
        return source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }
}
