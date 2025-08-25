namespace Test.Domain.Primitives
{
    public class PaginatedList<TEntity>
    {
        public IReadOnlyList<TEntity> Items { get; }

        public int Page { get; }

        public int PageSize { get; }

        public long TotalCount { get; }

        public PaginatedList(
            IReadOnlyList<TEntity> items, 
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
