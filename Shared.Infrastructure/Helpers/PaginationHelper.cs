using Marten;

namespace Shared.Infrastructure.Helpers;

public static class PaginationHelper
{
    public static async Task<PagedResult<T>> PaginateAsync<T>(
        IQueryable<T> query, int pageNumber = 1, 
        int pageSize = 10) where T : notnull
    {
        var result = new PagedResult<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = await query.CountAsync(),
            Items = (await query
                    .Skip((pageNumber) * pageSize)
                    .Take(pageSize)
                    .ToListAsync())
                .ToList()
        };

        return result;
    }
}

public class PagedResult<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public List<T> Items { get; set; } = new List<T>();
}