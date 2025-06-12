using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;
using TestRating.Application.Common.Interfaces;

namespace TestRating.Application.Behaviors
{
    public class CheckTestIsExistBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICheckTestIsExist
    {
        private readonly ITestExternalService testExternalService;

        public CheckTestIsExistBehavior(
            ITestExternalService testExternalService)
        {
            this.testExternalService = testExternalService;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var testIsExist = await testExternalService
                .TestIsExists(request.TestId, cancellationToken);
        
            if(!testIsExist)
            {
                throw new BadRequestException("Test doesnt exist");
            }

            return await next(cancellationToken);
        }
    }
}
