using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Domain.Interfaces
{
    public interface INoSQLUnitOfWork: IBaseUnitOfWork
    {
        IProfileRepository ProfileRepository { get; }

        ITestRepository TestRepository { get; }

        IQuestionRepository QuestionRepository { get; }

        IQuestionAnswerRepository QuestionAnswerRepository { get; }

        ITestSessionRepository SessionRepository { get; }

        IProfileAnswerRepository ProfileAnswerRepository { get; }

        IProfileGroupRepository ProfileGroupRepository { get; }

        ITestAccessRepository AccessRepository { get; }

        IOutboxMessageRepository OutboxMessageRepository { get; }

        IReadOnlyCollection<DomainEventEntity> GetTrackedEntities();

        // is used to add an entity to its event tracking
        void TrackEntity(DomainEventEntity entity);
    }
}
