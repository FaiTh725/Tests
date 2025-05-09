using Test.Dal.Adapters;
using Test.Dal.Repositories;
using Test.Domain.Interfaces;
using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Dal.Services
{
    public class UnitOfWork : INoSQLUnitOfWork
    {
        private readonly AppDbContext context;

        private readonly Lazy<IProfileRepository> profileRepository;
        private readonly Lazy<ITestRepository> testRepository;
        private readonly Lazy<IQuestionRepository> questionRepository;
        private readonly Lazy<IQuestionAnswerRepository> questionAnswerRepository;
        private readonly Lazy<IProfileAnswerRepository> profileAnswerRepository;
        private readonly Lazy<IProfileGroupRepository> profileGroupRepository;
        private readonly Lazy<ITestSessionRepository> testSessionRepository;
        private readonly Lazy<ITestAccessRepository> testAccessRepository;
        private readonly Lazy<IOutboxMessageRepository> outboxMessageRepository;

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
            outboxMessageRepository = new Lazy<IOutboxMessageRepository>(() => new OutboxMessageRepository(context));
        }

        public IProfileRepository ProfileRepository => profileRepository.Value;

        public ITestRepository TestRepository => testRepository.Value;

        public IQuestionRepository QuestionRepository => questionRepository.Value;

        public IQuestionAnswerRepository QuestionAnswerRepository => questionAnswerRepository.Value;

        public ITestSessionRepository SessionRepository => testSessionRepository.Value;

        public IProfileAnswerRepository ProfileAnswerRepository => profileAnswerRepository.Value;

        public IProfileGroupRepository ProfileGroupRepository => profileGroupRepository.Value;

        public ITestAccessRepository AccessRepository => testAccessRepository.Value;

        public IOutboxMessageRepository OutboxMessageRepository => outboxMessageRepository.Value;

        public IDatabaseSession BeginTransaction()
        {
            var mongoSession = context.Client.StartSession();
            mongoSession.StartTransaction();

            return new MongoSessionAdapter(mongoSession);
        }

        public async Task<IDatabaseSession> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var mongoSession = await context.Client.StartSessionAsync(cancellationToken: cancellationToken);
            mongoSession.StartTransaction();
        
            return new MongoSessionAdapter(mongoSession);
        }

        public void CommitTransaction(IDatabaseSession session)
        {
            var mongoSession = session as MongoSessionAdapter;
            AssuranceTransaction(mongoSession);

            mongoSession!.Session.CommitTransaction();
            mongoSession.CloseSession();
        }

        public async Task CommitTransactionAsync(
            IDatabaseSession session, 
            CancellationToken cancellationToken = default)
        {
            var mongoSession = session as MongoSessionAdapter;
            AssuranceTransaction(mongoSession);

            await mongoSession!.Session
                .CommitTransactionAsync(cancellationToken: cancellationToken);
            mongoSession.CloseSession();
        }

        public void RollBackTransaction(
            IDatabaseSession session)
        {
            var mongoSession = session as MongoSessionAdapter;
            AssuranceTransaction(mongoSession);

            mongoSession!.Session.AbortTransaction();
            mongoSession.CloseSession();
        }

        public async Task RollBackTransactionAsync(
            IDatabaseSession session, 
            CancellationToken cancellationToken = default)
        {
            var mongoSession = session as MongoSessionAdapter;
            AssuranceTransaction(mongoSession);

            await mongoSession!.Session
                .AbortTransactionAsync(cancellationToken: cancellationToken);
            mongoSession.CloseSession();
        }

        public void Dispose()
        {
        }

        private void AssuranceTransaction(MongoSessionAdapter? mongoSession)
        {
            if (!mongoSession?.Session.IsInTransaction != true)
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
