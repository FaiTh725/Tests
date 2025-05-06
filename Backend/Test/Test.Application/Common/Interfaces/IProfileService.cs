
using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Common.Interfaces
{
    public interface IProfileService
    {
        Task<ConfirmedProfile> DecodeToken(string? token, CancellationToken cancellationToken = default);
    }
}
