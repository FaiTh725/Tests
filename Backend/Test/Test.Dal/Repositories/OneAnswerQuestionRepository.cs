using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class OneAnswerQuestionRepository : IOneAnswerQuestionRepository
    {
        private readonly AppDbContext context;

        public OneAnswerQuestionRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<OneAnswerQuestion> AddOneAnswerQuestion(
            OneAnswerQuestion question, CancellationToken cancellationToken = default)
        {
            var mongoQuestion = new MongoOneAnswerQuestion();
            mongoQuestion.ConvertToMongoEntity(question);
            var nextId = context.GetNextId(AppDbContext.ONE_ANSWER_COLLECTION_NAME);
            mongoQuestion.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.OneAnswerQuestion
                .InsertOneAsync(mongoQuestion, insertOptions, cancellationToken);

            return mongoQuestion.ConvertToDomainEntity();
        }

        public async Task<OneAnswerQuestion?> GetOneAnswerQuestion(long id, CancellationToken cancellationToken = default)
        {
            var mongoQuestion = await context.OneAnswerQuestion
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoQuestion?.ConvertToDomainEntity();
        }
    }
}
