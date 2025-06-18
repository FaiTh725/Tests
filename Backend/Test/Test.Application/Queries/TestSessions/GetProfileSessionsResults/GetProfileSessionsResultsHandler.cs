using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.TestSession;
using Test.Application.Queries.Test.Specifications;
using Test.Application.Queries.TestSessions.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.TestSessions.GetProfileSessionsResults
{
    public class GetProfileSessionsResultsHandler :
        IRequestHandler<GetProfileSessionsResultsQuery, PaginationResponse<SessionResult>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileSessionsResultsHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<SessionResult>> Handle(
            GetProfileSessionsResultsQuery request, 
            CancellationToken cancellationToken)
        {
            var allProfileSessions = await unitOfWork.SessionRepository
                .GetSessionsByCriteria(new GetProfileSessionsSpecifications(
                    request.ProfileId), 
                    cancellationToken);
        
            var profileSessions = await unitOfWork.SessionRepository
                .GetSessionsByCriteria(new GetProfilePaginationSessionsSpecifications(
                    request.ProfileId, request.Page, request.PageSize),
                    cancellationToken);

            var testFinishedIdList = profileSessions
                .Select(x => x.TestId)
                .Distinct()
                .ToList();

            var allFinishedTests = await unitOfWork.TestRepository
                .GetTestsByCriteria(
                new GetTestsByIdListSpecification(testFinishedIdList), 
                cancellationToken);

            var finishedTestsNameDictinary = allFinishedTests
                .ToDictionary(
                    x => x.Id, 
                    x => x.Name);

            return new PaginationResponse<SessionResult>
            {
                Data = profileSessions.Select(x => new SessionResult
                { 
                    Id = x.Id,
                    Percent = x.Percent,
                    ProfileId = x.ProfileId,
                    TestId = x.TestId,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime!.Value,
                    TestName = finishedTestsNameDictinary.GetValueOrDefault(x.TestId, string.Empty)
                }),
                PageSize = request.PageSize,
                Page = request.Page,
                MaxSize = allProfileSessions.Count()
            };
        }
    }
}
