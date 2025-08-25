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

            var paginatedProfileTests = await unitOfWork.TestRepository
                .GetPaginatedTestsByCriteria(
                new TestsByProfileIdWithPaginationSpecification(
                    profile.Id, 
                    request.Page, 
                    request.PageCount),
                cancellationToken);

            return new PaginationResponse<TestInfo>
            {
                Data = paginatedProfileTests.Items.Select(x => new TestInfo
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
                }),
                MaxSize = paginatedProfileTests.TotalCount,
                Page = request.Page,
                PageSize = request.PageCount
            };
        }
    }
}
