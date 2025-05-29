using CSharpFunctionalExtensions;

namespace Test.Application.Common.Interfaces
{
    public interface ITokenService<TTokenObject>
    {
        Result<TTokenObject> DecodeToken(string token);
    }
}
