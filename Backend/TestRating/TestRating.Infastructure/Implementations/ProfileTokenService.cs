using Application.Shared.Exceptions;
using CSharpFunctionalExtensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Profile;
using TestRating.Domain.Interfaces;

namespace TestRating.Infrastructure.Implementations
{
    public class ProfileTokenService : ITokenService<ProfileToken>
    {
        private readonly IUnitOfWork unitOfWork;

        public ProfileTokenService(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public Result<ProfileToken> DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var email = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            var name = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var role = jwtToken.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;

            if (email is null ||
                name is null ||
                role is null)
            {
                return Result.Failure<ProfileToken>("Invalid token signature");
            }

            return Result.Success(new ProfileToken
            {
                Email = email,
                Name = name
            });
        }

        public async Task<DecodedProfile> VerifyToken(
            string? token,
            CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new UnauthorizeException("Token is empty");
            }

            var decodedToken = DecodeToken(token);

            if(decodedToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var profile = await unitOfWork.ProfileRepository
                .GetProfileByEmail(decodedToken.Value.Email, cancellationToken);

            if(profile is null)
            {
                throw new NotFoundException("Profile with email - " +
                    decodedToken.Value.Email + " doesnt exist");
            }

            return new DecodedProfile
            {
                Id = profile.Id,
                Email = profile.Email,
                Name = profile.Name,
                Role = decodedToken.Value.Role
            };
        }
    }
}
