using MongoDB.Driver;
using Test.Dal.Repositories;
using Test.Domain.Interfaces;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Services
{
    public class UnitOfWork : INoSQLUnitOfWork
    {
        private readonly AppDbContext context;
        private IClientSessionHandle session;

        private bool isTransactionStarted;

        private readonly Lazy<IProfileRepository> profileRepository;
        private readonly Lazy<ITestRepository> testRepository;
        private readonly Lazy<IQuestionRepository> questionRepository;
        private readonly Lazy<IQuestionAnswerRepository> questionAnswerRepository;
        private readonly Lazy<IProfileAnswerRepository> profileAnswerRepository;
        private readonly Lazy<IProfileGroupRepository> profileGroupRepository;
        private readonly Lazy<ITestSessionRepository> testSessionRepository;
        private readonly Lazy<ITestAccessRepository> testAccessRepository;

        private readonly List<DomainEventEntity> trackedEntities = new List<DomainEventEntity>();

        public UnitOfWork(
            AppDbContext context)
        {
            this.context = context;

            profileRepository = new Lazy<IProfileRepository>(() => new ProfileRepository(context));
            testRepository = new Lazy<ITestRepository>(() => new TestRepository(context));
            questionAnswerRepository = new Lazy<IQuestionAnswerRepository>(() => new QuestionAnswerRepository(context));
            questionRepository = new Lazy<IQuestionRepository>(() => new QuestionRepository(context));
            profileAnswerRepository = new Lazy<IProfileAnswerRepository>(() => new ProfileAnswerRepository(context));
            profileGroupRepository = new Lazy<IProfileGroupRepository>(() => new ProfileGroupRepository(context));
            testSessionRepository = new Lazy<ITestSessionRepository>(() => new TestSessionRepository(context));
            testAccessRepository = new Lazy<ITestAccessRepository>(() => new TestAccessRepository(context));
        }

        public IProfileRepository ProfileRepository => profileRepository.Value;

        public ITestRepository TestRepository => testRepository.Value;

        public IQuestionRepository QuestionRepository => questionRepository.Value;

        public IQuestionAnswerRepository QuestionAnswerRepository => questionAnswerRepository.Value;

        public ITestSessionRepository SessionRepository => testSessionRepository.Value;

        public IProfileAnswerRepository ProfileAnswerRepository => profileAnswerRepository.Value;

        public IProfileGroupRepository ProfileGroupRepository => profileGroupRepository.Value;

        public ITestAccessRepository AccessRepository => testAccessRepository.Value;

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
