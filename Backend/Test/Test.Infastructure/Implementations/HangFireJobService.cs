using Hangfire;
using System.Linq.Expressions;
using Test.Application.Common.Interfaces;

namespace Test.Infrastructure.Implementations
{
    public class HangFireJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient backgroundJobClient;
        private readonly IRecurringJobManager recurringJobManager;

        public HangFireJobService(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager)
        {
            this.backgroundJobClient = backgroundJobClient;
            this.recurringJobManager = recurringJobManager;
        }

        public void CancelJob(string jobId)
        {
            backgroundJobClient.Delete(jobId);
        }

        public string CreateDelayedJob<T>(Expression<Action<T>> methodCall, TimeSpan timeSpan)
        {
            return backgroundJobClient.Schedule(methodCall, timeSpan);
        }

        public string CreateDelayedJob<T>(Expression<Func<T, Task>> methodCall, TimeSpan timeSpan)
        {
            return backgroundJobClient.Schedule(methodCall, timeSpan);
        }

        public string CreateFireAndForgetJob<T>(Expression<Action<T>> methodCall)
        {
            return backgroundJobClient.Enqueue(methodCall);
        }

        public void CreateSchedulingJob<T>(string jobId, Expression<Func<T, Task>> methodCall, string cronExpression)
        {
            recurringJobManager.AddOrUpdate(jobId, methodCall, cronExpression);
        }
    }
}
