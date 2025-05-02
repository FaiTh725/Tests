using Grpc.Core;
using Test.API.Grpc;
using TestRating.Application.Common.Interfaces;

namespace TestRating.API.Grpc.Services
{
    public class TestExternalService :
        ITestExternalService
    {
        private readonly TestService.TestServiceClient testServiceClient;
        private readonly ILogger<TestExternalService> logger;

        public TestExternalService(
            TestService.TestServiceClient testServiceClient, 
            ILogger<TestExternalService> logger)
        {
            this.testServiceClient = testServiceClient;
            this.logger = logger;
        }

        public async Task<bool> TestIsExists(
            long testId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var isExistTestServiceResponse = await testServiceClient.TestIsExistsAsync(
                    new TestIsExistsRequest 
                    { 
                        TestId = testId
                    }, cancellationToken: cancellationToken);

                return isExistTestServiceResponse.IsExists;
            }
            catch(RpcException ex)
            {
                logger.LogError("Error when sending grpc a request to test service. " +
                    $"Error message - {ex.Message}");

                return false;
            }
        }
    }
}
