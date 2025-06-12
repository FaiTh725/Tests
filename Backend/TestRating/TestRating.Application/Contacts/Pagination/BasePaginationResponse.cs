namespace TestRating.Application.Contacts.Pagination
{
    public class BasePaginationResponse<TData>
    {
        public IEnumerable<TData> Items { get; set; } = Enumerable.Empty<TData>();

        public int Page {  get; set; }

        public int PageCount { get; set; }

        public int MaxCount { get; set; }
    }
}
