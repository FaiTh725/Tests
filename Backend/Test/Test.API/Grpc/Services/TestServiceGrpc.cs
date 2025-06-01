using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Test.Application.Queries.Test.GetTestInfoById;
using Test.Domain.Enums;
using Test.Domain.Interfaces;

namespace Test.API.Grpc.Services
{
    public class TestServiceGrpc : Grpc.TestService.TestServiceBase
    {
        private readonly INoSQLUnitOfWork unitOfWork;
        private readonly IMediator mediator;

        public TestServiceGrpc(
            INoSQLUnitOfWork unitOfWork,
            IMediator mediator)
        {
            this.unitOfWork = unitOfWork;   
            this.mediator = mediator;
        }

        public override async Task<TestIsExistsResponse> TestIsExists(
            TestIsExistsRequest request, 
            ServerCallContext context)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, context.CancellationToken);

            return new TestIsExistsResponse 
            { 
                IsExists = test is not null
            };
        }

        public override async Task<TestInfoResponse> GetTestInfoWithOwner(
            TestInfoRequest request, 
            ServerCallContext context)
        {
            var test = await mediator
                .Send(new GetTestInfoByIdQuery
                {
                    Id = request.TestId
                }, 
                context.CancellationToken);

            return new TestInfoResponse
            {
                Id = test.Id,
                IsPublic = test.IsPublic,
                Owner = new TestOwner
                {
                    Name = test.Owner.Name,
                    Email = test.Owner.Email,
                    Id = test.Owner.Id
                },
                Type = test.TestType == TestType.Timed.ToString() ? 0 : 1,
                DurationInMinutes = test.DurationInMinutes,
                CreatedTime = Timestamp.FromDateTime(test.CreatedTime),                
                Name = test.Name,
                Description = test.Description,
            };
        }
    }
}
