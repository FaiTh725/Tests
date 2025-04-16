using Application.Shared.Exceptions;
using Grpc.Core;
using MediatR;
using Test.Application.Commands.ProfileEntity.CreateProfile;
using Test.Application.Queries.ProfileEntity.GetProfileById;

namespace Test.API.Grpc.Services
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
            AddProfileRequest request, ServerCallContext context)
        {
            try
            {
                var profileId = await mediator.Send(new CreateProfileCommand
                {
                    Email = request.Email,
                    Name = request.Name,
                });

                var profile = await mediator.Send(new GetProfileByIdQuery
                {
                    Id = profileId,
                });

                return new AddProfileResponse 
                { 
                    Id = profile.Id,
                    Name = profile.Name,
                    Email = profile.Email
                };
            }
            catch (ApiException ex)
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument, 
                    ex.Message));
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Unknown erorr"));
            }
        }
    }
}
