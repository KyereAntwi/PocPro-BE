namespace DevSync.PocPro.Shops.Shared.Dtos;

public class PagedResponse<TResponse>  where TResponse : class
{
    public PagedResponse(int page, int pageSize, long totalCount, IEnumerable<TResponse> items)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;
    }

    public int Page { get; }
    public int PageSize { get; }
    public long TotalCount { get; }
    public IEnumerable<TResponse> Items { get; }
    public bool HasPrev => Page > 1;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
}