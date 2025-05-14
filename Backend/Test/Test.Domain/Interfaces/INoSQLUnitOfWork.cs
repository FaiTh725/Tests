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

        IReadOnlyCollection<DomainEventEntity> GetTrackedEntities();

        // it's used to add entities to the tracking
        void TrackEntity(DomainEventEntity entity);
    }
}
