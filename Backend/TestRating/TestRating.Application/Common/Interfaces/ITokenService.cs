using CSharpFunctionalExtensions;

namespace TestRating.Application.Common.Interfaces
{
    public interface ITokenService<TTokenObject>
    {
        Result<TTokenObject> DecodeToken(string token);
    }
}
