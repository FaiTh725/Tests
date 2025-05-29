using MassTransit.Initializers;
using Redis.OM;
using Redis.OM.Searching;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.TestSession;
using Test.Infrastructure.RedisEntities;

namespace Test.Infrastructure.Implementations
{
    public class RedisTempDbService : ITempDbService<TempTestSession>
    {
        private readonly RedisCollection<RedisTestSession> sessions;
        private readonly RedisConnectionProvider redisProvider;

        public RedisTempDbService(
            RedisConnectionProvider redisProvider)
        {
            this.redisProvider = redisProvider;
            sessions = (RedisCollection<RedisTestSession>)redisProvider
                .RedisCollection<RedisTestSession>();
        }

        public async Task<TempTestSession> AddEntity(TempTestSession testSession)
        {
            var redisTestSessionEntity = new RedisTestSession
            {
                Id = testSession.Id,
                Answers = [..testSession.Answers],
                JobId = testSession.JobId,
                ProfileId = testSession.ProfileId,
                StartTime = testSession.StartTime,
                TestDuration = testSession.TestDuration,
                TestId = testSession.TestId,
            };

            await sessions.InsertAsync(redisTestSessionEntity);

            return testSession;
        }

        public async Task<IEnumerable<TempTestSession>> GetAllEntities()
        {
            var redisTestSessions = await sessions.ToListAsync();

            return redisTestSessions.Select(x => new TempTestSession
            {
                Id = x.Id,
                JobId = x.JobId,
                ProfileId = x.ProfileId,
                StartTime = x.StartTime,
                TestDuration = x.TestDuration,
                Answers= x.Answers,
                TestId = x.TestId
            });
        }

        public async Task<TempTestSession?> GetEntity(Guid id)
        {
            var redisTestSessionEntity = await sessions
                .FindByIdAsync(id.ToString());

            return redisTestSessionEntity is null ? null : new TempTestSession
            {
                Id = redisTestSessionEntity.Id,
                Answers = [.. redisTestSessionEntity.Answers],
                JobId = redisTestSessionEntity.JobId,
                ProfileId = redisTestSessionEntity.ProfileId,
                StartTime = redisTestSessionEntity.StartTime,
                TestDuration = redisTestSessionEntity.TestDuration,
                TestId = redisTestSessionEntity.TestId
            };
        }

        public async Task RemoveEntity(Guid id)
        {
            await redisProvider.Connection.UnlinkAsync($"Sessions:{id}");
        }

        public async Task UpdateEntity(
            Guid sessionId, 
            TempTestSession updatedSession)
        {
            var session = await sessions
                .FindByIdAsync(sessionId.ToString());

            if (session is null)
            {
                return;
            }

            session.TestId = updatedSession.TestId;
            session.ProfileId = updatedSession.ProfileId;
            session.TestDuration = updatedSession.TestDuration;

            session.Answers.Clear();
            session.Answers.AddRange(updatedSession.Answers);

            await sessions.SaveAsync();
        }
    }
}
