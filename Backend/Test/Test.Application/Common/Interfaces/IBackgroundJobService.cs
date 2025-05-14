using System.Linq.Expressions;

namespace Test.Application.Common.Interfaces
{
    public interface IBackgroundJobService
    {
        string CreateFireAndForgetJob<T>(Expression<Action<T>> methodCall);

        string CreateDelayedJob<T>(Expression<Action<T>> methodCall, TimeSpan timeSpan);

        string CreateDelayedJob<T>(Expression<Func<T, Task>> methodCall, TimeSpan timeSpan);

        void CreateSchedulingJob<T>(string jobId, Expression<Func<T, Task>> methodCall, string cronExpression);

        void CancelJob(string jobId);
    }
}
