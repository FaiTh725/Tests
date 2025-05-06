using Application.Shared.Exceptions;
using Authorization.Application.Commands.UserEntity.Register;
using Authorization.Application.Common.Interfaces;
using MediatR;

namespace Authorization.Application.Behaviors
{
    public class RegistrationAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : RegisterCommand
    {
        private readonly ICacheService cacheService;

        public RegistrationAccessBehavior(
            ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var comfirmedEmail = await cacheService
                .GetData<string>(
                "confirmed_email:" + request.Email, 
                cancellationToken);

            if(comfirmedEmail.IsFailure)
            {
                throw new ForbiddenAccessException("Confirm email before registration");
            }

            return await next(cancellationToken);
        }
    }
}
