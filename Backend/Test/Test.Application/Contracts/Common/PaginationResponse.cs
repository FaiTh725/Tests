using System.Text;

namespace Test.Application.Contracts.Common
{
    public class PaginationResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public long MaxSize { get; set; }
    }
}
