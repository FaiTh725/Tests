using MongoDB.Driver;
using Test.Dal.Repositories;
using Test.Domain.Intrefaces;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Services
{
    public class UnitOfWork : INoSQLUnitOfWork
    {
        private readonly AppDbContext context;
        private IClientSessionHandle session;

        private bool isTransactionStarted;

        private Lazy<IProfileRepository> profileRepository;
        private Lazy<ITestRepository> testRepository;
        private Lazy<IQuestionRepository> questionRepository;
        private Lazy<IQuestionAnswerRepository> questionAnswerRepository;

        private List<DomainEventEntity> trackedEntities = new List<DomainEventEntity>();

        public UnitOfWork(
            AppDbContext context)
        {
            this.context = context;

            profileRepository = new Lazy<IProfileRepository>(() => new ProfileRepository(context));
            testRepository = new Lazy<ITestRepository>(() => new TestRepository(context));
            questionAnswerRepository = new Lazy<IQuestionAnswerRepository>(() => new QuestionAnswerRepository(context));
            questionRepository = new Lazy<IQuestionRepository>(() => new QuestionRepository(context));
        }

        public IProfileRepository ProfileRepository => profileRepository.Value;

        public ITestRepository TestRepository => testRepository.Value;

        public IQuestionRepository QuestionRepository => questionRepository.Value;

        public IQuestionAnswerRepository QuestionAnswerRepository => questionAnswerRepository.Value;

        public void BeginTransaction()
        {
            session = context.Client.StartSession();
            session.StartTransaction();

            isTransactionStarted = true;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            session = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            session.StartTransaction();

            isTransactionStarted = true;
        }

        public void CommitTransaction()
        {
            AssuranceTransaction();

            session.CommitTransaction();

            isTransactionStarted = false;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            AssuranceTransaction();

            await session.CommitTransactionAsync(cancellationToken: cancellationToken);

            isTransactionStarted = false;
        }


        public void RollBackTransaction()
        {
            AssuranceTransaction();

            session.AbortTransaction();
        }

        public async Task RollBackTransactionAsync(CancellationToken cancellationToken = default)
        {
            AssuranceTransaction();

            await session.AbortTransactionAsync(cancellationToken: cancellationToken);
        }

        public void Dispose()
        {
            session?.Dispose();
        }

        private void AssuranceTransaction()
        {
            if(!isTransactionStarted ||
                session is null)
            {
                throw new InvalidOperationException("Transaction is not started");
            }
        }

        public IReadOnlyCollection<DomainEventEntity> GetTrackedEntities()
        {
            return trackedEntities.AsReadOnly();
        }

        public void TrackEntity(DomainEventEntity entity)
        {
            trackedEntities.Add(entity);
        }
    }
}
