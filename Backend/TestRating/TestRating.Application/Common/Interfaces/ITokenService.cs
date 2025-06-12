using CSharpFunctionalExtensions;
using TestRating.Application.Contacts.Profile;

namespace TestRating.Application.Common.Interfaces
{
    public interface ITokenService<TTokenObject>
    {
        Result<TTokenObject> DecodeToken(string token);

        Task<DecodedProfile> VerifyToken(string? token, CancellationToken cancellationToken = default);
    }
}
