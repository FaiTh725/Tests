using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.ProfileEntity.Specifications;
using Test.Application.Queries.Test.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.Test.GetTests
{
    public class GetTestsHandler :
        IRequestHandler<GetTestsQuery, PaginationResponse<TestInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetTestsHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<TestInfo>> Handle(
            GetTestsQuery request, 
            CancellationToken cancellationToken)
        {
            var paginatedTests = await unitOfWork.TestRepository
                .GetPaginatedTestsByCriteria(
                new GetTestsPaginationSpecification(
                    request.Page, 
                    request.PageSize), 
                cancellationToken);

            var ownerId = paginatedTests.Items
                .Select(x => x.ProfileId)
                .Distinct()
                .ToList();

            var owners = await unitOfWork.ProfileRepository
                .GetProfilesByCriteria(
                    new GetProfilesByIdListSpecification(ownerId), 
                cancellationToken);

            var ownersDictinary = owners.ToDictionary(
                x => x.Id, 
                x => new ProfileResponse
                {
                    Id = x.Id,
                    Email = x.Email,
                    Name = x.Name
                });

            return new PaginationResponse<TestInfo> 
            {
                Data = paginatedTests.Items.Select(x => new TestInfo
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedTime = x.CreatedTime,
                    Description = x.Description,
                    DurationInMinutes = x.DurationInMinutes,
                    IsPublic = x.IsPublic,
                    TestType = x.TestType.ToString(),
                    Owner = ownersDictinary[x.ProfileId],
                }),
                MaxSize = paginatedTests.TotalCount,
                Page = request.Page,
                PageSize = request.PageSize,
            };
        }
    }
}
