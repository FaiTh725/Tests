using System.Linq.Expressions;

namespace Test.Application.Common.Interfaces
{
    public interface IBackgroundJobService
    {
        string CreateFireAndForgetJob<T>(Expression<Action<T>> methodCall);

        string CreateDelaydedJob<T>(Expression<Action<T>> methodCall, TimeSpan timeSpan);

        string CreateDelaydedJob<T>(Expression<Func<T, Task>> methodCall, TimeSpan timeSpan);

        void CancelJob(string jobId);
    }
}
