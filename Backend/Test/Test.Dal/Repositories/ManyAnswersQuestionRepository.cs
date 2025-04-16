using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class ManyAnswersQuestionRepository : IManyAnswersQuestionRepository
    {
        private readonly AppDbContext context;

        public ManyAnswersQuestionRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<ManyAnswersQuestion> AddManyAnswerQuestion(
            ManyAnswersQuestion question, CancellationToken cancellationToken = default)
        {
            var mongoQuestion = new MongoManyAnswerQuestion();
            mongoQuestion.ConvertToMongoEntity(question);
            var nextId = context.GetNextId(AppDbContext.MANY_ANSWERS_COLLECTION_NAME);
            mongoQuestion.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.ManyAnswerQuestions
                .InsertOneAsync(mongoQuestion, insertOptions, cancellationToken);

            return mongoQuestion.ConvertToDomainEntity();
        }

        public async Task<ManyAnswersQuestion?> GetManyAnswersQuestion(
            long id, CancellationToken cancellationToken = default)
        {
            var mongoQuestion = await context.ManyAnswerQuestions
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoQuestion?.ConvertToDomainEntity();
        }
    }
}
