
using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Common.Interfaces
{
    public interface IProfileService
    {
        ProfileToken VerifyProfileFromToken(string? token);

        Task<VerifiedProfile> DecodeProfileFromToken(string? token, CancellationToken cancellationToken = default);
    }
}
