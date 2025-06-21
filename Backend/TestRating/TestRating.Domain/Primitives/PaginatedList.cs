namespace TestRating.Domain.Primitives
{
    public class PaginatedList<TEntity>
    {
        public List<TEntity> Items { get; }

        public int Page { get; }

        public int PageSize { get; }

        public long TotalCount { get; }

        public PaginatedList(
            List<TEntity> items,
            int page,
            int pageSize,
            long totalCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
