using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.Test.Specifications;
using Test.Domain.Entities;
using Test.Domain.Intrefaces;

namespace Test.Application.Queries.Test.GetProfileTests
{
    public class GetProfileTestsHandler :
        IRequestHandler<GetProfileTestsQuery, IEnumerable<TestInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileTestsHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<TestInfo>> Handle(
            GetProfileTestsQuery request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileId, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var profileTests = await unitOfWork.TestRepository
                .GetTestsByCriteria(
                new TestsByProfileIdSpecification(profile.Id),
                cancellationToken);

            return profileTests.Select(x => new TestInfo
            {
                Id = x.Id,
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
        }
    }
}
