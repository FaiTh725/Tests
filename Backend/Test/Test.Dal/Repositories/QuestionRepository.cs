using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Test.Dal.ExpressionRewriters;
using Test.Dal.Persistences;
using Test.Domain.Entities;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly AppDbContext context;

        public QuestionRepository(
            AppDbContext context)
        {
            this.context = context;
        }

        public async Task<Question> AddQuestion(
            Question question, CancellationToken cancellationToken = default)
        {
            var mongoQuestion = new MongoQuestion();
            mongoQuestion.ConvertToMongoEntity(question);
            var nextId = context.GetNextId(AppDbContext.QUESTION_COLLECTION_NAME);
            mongoQuestion.Id = nextId;

            var insertOptions = new InsertOneOptions
            {
                BypassDocumentValidation = true
            };

            await context.Questions
                .InsertOneAsync(mongoQuestion, insertOptions, cancellationToken);
            
            return mongoQuestion.ConvertToDomainEntity();
        }

        public async Task DeleteQuestions(List<long> questionIdList, CancellationToken cancellationToken = default)
        {
            await context.Questions
                .DeleteManyAsync(x => questionIdList.Contains(x.Id), 
                cancellationToken);
        }

        public async Task<Question?> GetQuestion(
            long id, CancellationToken cancellationToken = default)
        {
            var question = await context.Questions
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return question?.ConvertToDomainEntity();
        }

        public async Task<IEnumerable<Question>> GetQuestionsByCriteria(
            BaseSpecification<Question> specification, 
            CancellationToken cancellationToken = default)
        {
            var filter = specification.Criteria is null ?
                Builders<MongoQuestion>.Filter.Empty :
                new QuestionToMongoRewriter().Rewrite(specification.Criteria);

            var mongoQuestions = await context.Questions
                .Find(filter)
                .ToListAsync(cancellationToken);

            return mongoQuestions
                .Select(x => x.ConvertToDomainEntity());
        }
    }
}
