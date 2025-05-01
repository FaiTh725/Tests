using Authorization.Application.Common.Interfaces;
using CSharpFunctionalExtensions;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Test.API.Grpc;
using TestRating.Contracts.Profile;

namespace Authorization.API.Grpc.Clients
{
    // TODO: delete
    public class FeedbackProfileClient :
        IExternalService<CreateProfileRequest, CreateProfileResponse>
    {
        private readonly ProfileService.ProfileServiceClient client;

        public FeedbackProfileClient(
            GrpcClientFactory grpcClientFactory)
        {
            client = grpcClientFactory.CreateClient<ProfileService.ProfileServiceClient>("FeedbackClient");
        }

        public async Task<Result<CreateProfileResponse>> GetData(CreateProfileRequest request)
        {
            try
            {
                var profile = await client.AddProfileAsync(new AddProfileRequest
                {
                    Email = request.Email,
                    Name = request.Name
                });

                return Result.Success(new CreateProfileResponse
                {
                    Id = profile.Id,
                    Email = request.Email,
                    Name = request.Name
                });
            }
            catch(RpcException ex)
            {
                return Result.Failure<CreateProfileResponse>("Error while calling external service." +
                    $" Status code - {ex.Status}." +
                    $" Error Message - {ex.Message}.");
            }
        }
    }
}
