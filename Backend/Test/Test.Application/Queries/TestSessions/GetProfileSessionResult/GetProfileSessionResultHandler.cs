using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.TestSession;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.TestSessions.GetProfileSessionResult
{
    public class GetProfileSessionResultHandler :
        IRequestHandler<GetProfileSessionResultQuery, SessionResult>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileSessionResultHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<SessionResult> Handle(
            GetProfileSessionResultQuery request, 
            CancellationToken cancellationToken)
        {
            var session = await unitOfWork.SessionRepository
                .GetFinishedTest(request.Id, cancellationToken);

            if (session is null)
            {
                throw new NotFoundException("Session doesnt exist");
            }

            var test = await unitOfWork.TestRepository
                .GetTest(session.TestId, cancellationToken);

            if (test is null)
            {
                throw new InternalServerErrorException("Session with a nonexistent test");
            }

            return new SessionResult 
            { 
                Id = session.Id,
                EndTime = session.EndTime!.Value,
                StartTime = session.StartTime,
                Percent = session.Percent,
                ProfileId = session.ProfileId,
                TestId = session.TestId,
                TestName = test.Name
            };
        }
    }
}
