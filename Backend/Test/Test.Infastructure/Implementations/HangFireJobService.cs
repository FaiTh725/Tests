using Hangfire;
using System.Linq.Expressions;
using Test.Application.Common.Interfaces;

namespace Test.Infastructure.Implementations
{
    public class HangFireJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient backgroundJobClient;

        public HangFireJobService(
            IBackgroundJobClient backgroundJobClient)
        {
            this.backgroundJobClient = backgroundJobClient;
        }

        public void CancelJob(string jobId)
        {
            backgroundJobClient.Delete(jobId);
        }

        public string CreateDelaydedJob<T>(Expression<Action<T>> methodCall, TimeSpan timeSpan)
        {
            return backgroundJobClient.Schedule(methodCall, timeSpan);
        }

        public string CreateFireAndForgetJob<T>(Expression<Action<T>> methodCall)
        {
            return backgroundJobClient.Enqueue(methodCall);
        }
    }
}
