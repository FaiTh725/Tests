using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class QuestionAnswerRepository : IQuestionAnswerRepository
    {
        private readonly AppDbContext context;

        public QuestionAnswerRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<QuestionAnswer> AddQuestionAnswer(
            QuestionAnswer questionAnswer, 
            CancellationToken cancellationToken = default)
        {
            var mongoQuestionAnswer = new MongoQuestionAnswer();
            mongoQuestionAnswer.ConvertToMongoEntity(questionAnswer);
            var nextId = context.GetNextId(AppDbContext.ANSWER_COLLECTION_NAME);
            mongoQuestionAnswer.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.Answers
                .InsertOneAsync(mongoQuestionAnswer, insertOptions, cancellationToken);
        
            return questionAnswer;
        }

        public async Task<QuestionAnswer> GetQuestionAnswer(
            long id, 
            CancellationToken cancellationToken = default)
        {
            var mongoQuestionAnswer = await context.Answers
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoQuestionAnswer?.ConvertToDomainEntity();
        }
    }
}
