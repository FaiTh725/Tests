using Grpc.Core;
using Test.Domain.Interfaces;

namespace Test.API.Grpc.Services
{
    public class TestServiceGrpc : Grpc.TestService.TestServiceBase
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public TestServiceGrpc(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;   
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
    }
}
