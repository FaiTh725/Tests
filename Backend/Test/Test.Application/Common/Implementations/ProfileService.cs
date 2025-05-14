using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.GetProfileByEmail;
using Test.Domain.Entities;

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

        public async Task<ProfileResponse> DecodeProfileFromToken(
            string? token,
            CancellationToken cancellationToken = default)
        {
            var profileToken = VerifyProfileFromToken(token);

            var profile = await mediator.Send(new GetProfileByEmailQuery
            {
                Email = profileToken.Email
            }, 
            cancellationToken);

            return profile;
        }

        public ProfileToken VerifyProfileFromToken(
            string? token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("User isnt authorized");
            }

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            return profileToken.Value;
        }
    }
}
