using Authorization.Application.Common.Interfaces;
using CSharpFunctionalExtensions;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Test.API.Grpc;
using Test.Contracts.Profile;

namespace Authorization.API.Grpc.Clients
{
    // TODO: delete
    public class TestingProfileClient :
        IExternalService<ProfileRequest, ProfileResponse>
    {
        private readonly ProfileService.ProfileServiceClient client;

        public TestingProfileClient(
            GrpcClientFactory grpcClientFactory)
        {
            client = grpcClientFactory.CreateClient<ProfileService.ProfileServiceClient>("TestingClient");
        }

        public async Task<Result<ProfileResponse>> GetData(ProfileRequest request)
        {
            try
            {
                var profile = await client.AddProfileAsync(new AddProfileRequest
                {
                    Email = request.Email,
                    Name = request.Name
                });

                return Result.Success(new ProfileResponse
                {
                    Id = profile.Id,
                    Email = request.Email,
                    Name = request.Name
                });
            }
            catch(RpcException ex)
            {
                return Result.Failure<ProfileResponse>("Error while calling external service." +
                    $" Status code - {ex.Status}." +
                    $" Error Message - {ex.Message}.");
            }
        }
    }
}
