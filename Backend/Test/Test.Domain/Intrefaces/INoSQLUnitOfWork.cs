﻿using Test.Domain.Primitives;
using Test.Domain.Repositories;

namespace Test.Domain.Intrefaces
{
    public interface INoSQLUnitOfWork: IBaseUnitOfWork
    {
        IProfileRepository ProfileRepository { get; }

        ITestRepository TestRepository { get; }

        IQuestionRepository QuestionRepository { get; }

        IQuestionAnswerRepository QuestionAnswerRepository { get; }

        IReadOnlyCollection<DomainEventEntity> GetTrackedEntities();

        // is used to add an entity to its event tracking
        void TrackEntity(DomainEventEntity entity);
    }
}
