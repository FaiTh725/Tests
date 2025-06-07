using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.Test.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.Test.GetProfileTests
{
    public class GetProfileTestsHandler :
        IRequestHandler<GetProfileTestsQuery, PaginationResponse<TestInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileTestsHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<PaginationResponse<TestInfo>> Handle(
            GetProfileTestsQuery request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileEmail, cancellationToken);

            if(profile is null)
            {
                throw new NotFoundException("Profile doesnt exist");
            }

            var allTests = await unitOfWork.TestRepository
                .GetTestsByCriteria(
                new TestsByProfileIdWithSpecification(profile.Id),
                cancellationToken);

            var profileTests = await unitOfWork.TestRepository
                .GetTestsByCriteria(
                new TestsByProfileIdWithPaginationSpecification(profile.Id, request.Page, request.PageCount),
                cancellationToken);

            var tests = profileTests.Select(x => new TestInfo
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedTime = x.CreatedTime,
                    Description = x.Description,
                    IsPublic = x.IsPublic,
                    TestType = x.TestType.ToString(),
                    DurationInMinutes = x.DurationInMinutes,
                    Owner = new ProfileResponse
                    {
                        Id = profile.Id,
                        Email = profile.Email,
                        Name = profile.Name
                    }
                });

            return new PaginationResponse<TestInfo>
            {
                Data = tests,
                MaxSize = allTests.Count(),
                Page = request.Page,
                PageSize = request.PageCount
            };
        }
    }
}
