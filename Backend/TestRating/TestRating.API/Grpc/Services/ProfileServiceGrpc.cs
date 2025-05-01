using Application.Shared.Exceptions;
using Grpc.Core;
using MediatR;
using TestRating.Application.Commands.ProfileEntity.AddProfile;
using TestRating.Application.Queries.ProfileEntity.GetProfileById;

namespace TestRating.API.Grpc.Services
{
    public class ProfileServiceGrpc: Grpc.ProfileService.ProfileServiceBase
    {
        private readonly IMediator mediator;

        public ProfileServiceGrpc(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        public override async Task<AddProfileResponse> AddProfile(
            AddProfileRequest request, 
            ServerCallContext context)
        {
            try
            {
                var profileId = await mediator.Send(new AddProfileCommand 
                {
                    Email = request.Email,
                    Name = request.Name
                }, context.CancellationToken);

                var createdProfile = await mediator.Send(new GetProfileByIdQuery
                {
                    Id = profileId
                }, context.CancellationToken);

                return new AddProfileResponse
                {
                    Id = createdProfile.Id,
                    Email = createdProfile.Email,
                    Name = createdProfile.Name
                };
            }
            catch (ApiException ex)
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    ex.Message));
            }
            catch (Exception)
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Unknown error"));
            }
        }
    }
}
