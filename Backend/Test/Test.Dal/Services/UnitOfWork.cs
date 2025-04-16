using MongoDB.Driver;
using Test.Dal.Repositories;
using Test.Domain.Intrefaces;
using Test.Domain.Repositories;

namespace Test.Dal.Services
{
    public class UnitOfWork : INoSQLUnitOfWork
    {
        private readonly AppDbContext context;
        private IClientSessionHandle session;

        private bool isTransactionStarted;

        private Lazy<IProfileRepository> profileRepository;

        public UnitOfWork(
            AppDbContext context)
        {
            this.context = context;

            profileRepository = new Lazy<IProfileRepository>(() => new ProfileRepository(context));
        }

        public IProfileRepository ProfileRepository => profileRepository.Value;

        public ITestRepository TestRepository => throw new NotImplementedException();

        public IManyAnswersQuestionRepository ManyAnswersQuestionRepository => throw new NotImplementedException();

        public IOneAnswerQuestionRepository OneAnswerQuestionRepository => throw new NotImplementedException();

        public IQuestionAnswerRepository QuestionAnswerRepository => throw new NotImplementedException();

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
    }
}
