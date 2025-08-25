using CSharpFunctionalExtensions;
using Grpc.Core;
using Test.API.Grpc;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Contacts.Test;

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

        public async Task<Result<TestInfo>> GetTest(
            long testId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var testIsExistResponse = await testServiceClient
                    .TestIsExistsAsync(new TestIsExistsRequest
                    {
                        TestId = testId
                    }, 
                    cancellationToken: cancellationToken);

                if(!testIsExistResponse.IsExists)
                {
                    return Result.Failure<TestInfo>("Test doesnt exists");
                }

                var testInfoResponse = await testServiceClient
                    .GetTestInfoWithOwnerAsync(new TestInfoRequest
                    {
                        TestId = testId
                    }, cancellationToken: cancellationToken);

                return Result.Success(
                    new TestInfo 
                    { 
                        Id = testInfoResponse.Id,
                        Name = testInfoResponse.Name,
                        IsPublic = testInfoResponse.IsPublic,
                        DurationInMinutes = testInfoResponse.DurationInMinutes,
                        Description = testInfoResponse.Description,
                        CreatedTime = testInfoResponse.CreatedTime.ToDateTime(),
                        TestType = testInfoResponse.Type == 0 ? "Timed" : "Progressive",
                        Owner = new BaseProfileResponse()
                        {
                            Id = testInfoResponse.Owner.Id,
                            Email = testInfoResponse.Owner.Email,
                            Name = testInfoResponse.Owner.Name
                        }
                    });
            }
            catch(RpcException)
            {
                return Result.Failure<TestInfo>("Error with calling external service");
            }
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
