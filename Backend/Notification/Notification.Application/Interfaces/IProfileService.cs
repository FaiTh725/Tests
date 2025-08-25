using CSharpFunctionalExtensions;
using Notification.Application.DTOs.Profiles;

namespace Notification.Application.Interfaces
{
    public interface IProfileService
    {
        Result<ProfileDTO> DecodeToken(string? token);
    }
}
