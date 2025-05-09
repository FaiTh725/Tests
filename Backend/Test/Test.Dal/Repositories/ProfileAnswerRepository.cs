using Test.Dal.Adapters;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class ProfileAnswerRepository : IProfileAnswerRepository
    {
        private readonly AppDbContext context;

        public ProfileAnswerRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<ProfileAnswer>> AddProfileAnswers(
            List<ProfileAnswer> profileAnswers, 
            IDatabaseSession? session = null,
            CancellationToken cancellationToken = default)
        {
            var mongoProfileAnswers = profileAnswers.Select(x =>
            {
                var mongoProfileAnswer = new MongoProfileAnswer();
                mongoProfileAnswer.ConvertToMongoEntity(x);
                var nextId = context.GetNextId(AppDbContext.PROFILE_ANSWERS_COLLECTION_NAME);
                mongoProfileAnswer.Id = nextId;
                
                return mongoProfileAnswer;
            });

            var mongoSession = (session as MongoSessionAdapter)?.Session;
            if (mongoSession is null)
            {
                await context.ProfileAnswers.InsertManyAsync(
                mongoProfileAnswers,
                cancellationToken: cancellationToken);
            }
            else
            {
                await context.ProfileAnswers.InsertManyAsync(
                    mongoSession,
                    mongoProfileAnswers,
                    cancellationToken: cancellationToken);
            }

            return mongoProfileAnswers.Select(x => x.ConvertToDomainEntity());
        }
    }
}
