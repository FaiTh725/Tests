using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.GetProfileByEmail;

namespace Test.Application.Common.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly ITokenService<ProfileToken> tokenService;
        private readonly IMediator mediator;

        public ProfileService(
            ITokenService<ProfileToken> tokenService,
            IMediator mediator)
        {
            this.tokenService = tokenService;
            this.mediator = mediator;
        }

        public async Task<ConfirmedProfile> DecodeToken(
            string? token, 
            CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("User isnt authorized");
            }

            var tokenPayload = tokenService.DecodeToken(token);

            if(tokenPayload.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var profile = await mediator.Send(new GetProfileByEmailQuery
            { 
                Email = tokenPayload.Value.Email
            }, cancellationToken);

            return new ConfirmedProfile
            {
                Id = profile.Id,
                Email = profile.Email,
                Name = profile.Name,
                Role = tokenPayload.Value.Role
            };
        }
    }
}
