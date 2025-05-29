using Authorization.Application.Common.Interfaces;
using CSharpFunctionalExtensions;
using Grpc.Core;
using Test.API.Grpc;
using Test.Contracts.Profile;

namespace Authorization.API.Grpc.Clients
{
    public class ProfileClient :
        IExternalService<ProfileRequest, ProfileResponse>
    {
        private readonly ProfileService.ProfileServiceClient client;

        public ProfileClient(
            ProfileService.ProfileServiceClient client)
        {
            this.client = client;
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
                return Result.Failure<ProfileResponse>("Error while calling externa service." +
                    $" Status code - {ex.Status}." +
                    $" Error Message - {ex.Message}.");
            }
        }
    }
}
