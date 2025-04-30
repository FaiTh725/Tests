using MongoDB.Driver;
using Test.Dal.Persistences;
using Test.Dal.Specifications;
using Test.Domain.Entities;
using Test.Domain.Primitives;
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
        
            return mongoQuestionAnswer.ConvertToDomainEntity();
        }

        public async Task<IEnumerable<QuestionAnswer>> AddQuestionAnswers(
            List<QuestionAnswer> questionAnswers, CancellationToken cancellationToken = default)
        {
            var mongoQuestions = questionAnswers.Select(x =>
            {
                var questionAnswer = new MongoQuestionAnswer();
                questionAnswer.ConvertToMongoEntity(x);
                var nextId = context.GetNextId(AppDbContext.ANSWER_COLLECTION_NAME);
                questionAnswer.QuestionId = nextId;
                return questionAnswer;
            });

            var insertQuestion = new InsertManyOptions
            {
                BypassDocumentValidation = true,
                IsOrdered = false,
            };

            await context.Answers.InsertManyAsync(
                mongoQuestions, insertQuestion, cancellationToken);

            return mongoQuestions
                .Select(x => x.ConvertToDomainEntity())
                .AsEnumerable();
        }

        public async Task DeleteAnswers(List<long> idList, CancellationToken cancellationToken = default)
        {
            await context.Answers
                .DeleteManyAsync(x => idList.Contains(x.Id), cancellationToken);
        }

        public async Task<QuestionAnswer?> GetQuestionAnswer(
            long id, 
            CancellationToken cancellationToken = default)
        {
            var mongoQuestionAnswer = await context.Answers
                .Find(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            return mongoQuestionAnswer?.ConvertToDomainEntity();
        }

        public async Task<IEnumerable<QuestionAnswer>> GetQuestionAnswersByCriteria(BaseSpecification<QuestionAnswer> specification, CancellationToken cancellationToken = default)
        {
            var filter = specification.Criteria is null ?
                Builders<MongoQuestionAnswer>.Filter.Empty :
                new ExpressionConverter<QuestionAnswer, MongoQuestionAnswer>().Rewrite(specification.Criteria);

            var answers = await context.Answers
                .Find(filter)
                .ToListAsync(cancellationToken);

            return answers.Select(x => x.ConvertToDomainEntity());
        }
    }
}
