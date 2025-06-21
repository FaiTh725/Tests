using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Application.Common.Interfaces;

namespace Test.Application.Behaviors
{
    public class GetCachedDataBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICachedData
    {
        private readonly ICacheService cacheService;

        public GetCachedDataBehavior(
            ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var cacheDataResult = await cacheService
                .GetData<TResponse>(request.Key, cancellationToken);
        
            if(cacheDataResult.IsSuccess)
            {
                return cacheDataResult.Value;
            }

            var result = await next(cancellationToken);

            await cacheService.SetData(request.Key, result, request.LifeTime, cancellationToken);

            return result;
        }
    }
}
