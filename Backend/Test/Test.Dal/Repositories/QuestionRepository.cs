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

        public async Task DeleteQuestion(long id, CancellationToken cancellationToken = default)
        {
            await context.Questions.DeleteManyAsync(x => x.Id == id, 
                cancellationToken);
        }

        public async Task DeleteQuestions(List<long> questionIdList, 
            CancellationToken cancellationToken = default)
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

        public async Task<Dictionary<long, IEnumerable<QuestionAnswer>>> GetQuestionCorrectAnswers(
            long testId, 
            CancellationToken cancellationToken = default)
        {
            var questionsId = await context.Questions
                .Find(x => x.TestId == testId)
                .Project(x => x.Id)
                .ToListAsync(cancellationToken);

            var correctAnswersQuestion = await context.Answers
                .Find(x => questionsId.Contains(x.QuestionId) && x.IsCorrect)
                .ToListAsync(cancellationToken);

            return correctAnswersQuestion
                .GroupBy(x => x.QuestionId)
                .ToDictionary(x => x.Key, x => x.Select(y => 
                    y.ConvertToDomainEntity()));
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

        public async Task UpdateQuestion(long id, Question updatedQuestion, 
            CancellationToken cancellationToken = default)
        {
            var mongoQuestion = new MongoQuestion();
            mongoQuestion.ConvertToMongoEntity(updatedQuestion);

            var filter = Builders<MongoQuestion>.Filter
                .Eq(x => x.Id, id);

            var update = Builders<MongoQuestion>.Update
                .Set(x => x.TestQuestion, updatedQuestion.TestQuestion)
                .Set(x => x.QuestionWeight, updatedQuestion.QuestionWeight);

            await context.Questions.UpdateOneAsync(filter, update, 
                cancellationToken: cancellationToken);
        }
    }
}
