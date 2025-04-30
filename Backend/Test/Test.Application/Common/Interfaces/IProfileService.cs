
using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Common.Interfaces
{
    public interface IProfileService
    {
        ProfileToken VerifyProfileFromToken(string? token);

        Task<ProfileResponse> DecodeProfileFromToken(string? token, CancellationToken cancellationToken = default);
    }
}
