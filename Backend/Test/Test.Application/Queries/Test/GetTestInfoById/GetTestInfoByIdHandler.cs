using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Domain.Intrefaces;

namespace Test.Application.Queries.Test.GetTestInfoById
{
    public class GetTestInfoByIdHandler :
        IRequestHandler<GetTestInfoByIdQuery, TestInfo>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetTestInfoByIdHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TestInfo> Handle(GetTestInfoByIdQuery request, CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.Id, cancellationToken);

            if(test is null)
            {
                throw new NotFoundException("Test doesnt exist");
            }

            var testOwner = await unitOfWork.ProfileRepository
                .GetProfile(test.ProfileId, cancellationToken);

            if(testOwner is null)
            {
                throw new InternalServerErrorException("Test without owner");
            }

            return new TestInfo
            {
                Id = test.Id,
                Name = test.Name,
                CreatedTime = test.CreatedTime,
                IsPublic = test.IsPublic,
                Description = test.Description,
                TestType = test.TestType.ToString(),
                DurationInMinutes = test.DurationInMinutes,
                Owner = new ProfileResponse
                { 
                    Id = testOwner.Id,
                    Name = testOwner.Name,
                    Email = testOwner.Email
                }
            };
        }
    }
}
