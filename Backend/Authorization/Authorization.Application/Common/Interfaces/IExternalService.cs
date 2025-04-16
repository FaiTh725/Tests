using CSharpFunctionalExtensions;

namespace Authorization.Application.Common.Interfaces
{
    public interface IExternalService<TRequest, TResponse>
    {
        Task<Result<TResponse>> GetData(TRequest request);
    }
}
