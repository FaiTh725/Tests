using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;
using Test.Domain.Intrefaces;

namespace Test.Application.Behaviors
{
    public class TestAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : ITestVisibleAccess
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public TestAccessBehavior(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);
        
            if(test is null)
            {
                throw new BadRequestException("Test doesnt exist");
            }

            if(test.IsPublic)
            {
                return await next(cancellationToken);
            }

            // TODO
            return await next(cancellationToken);
        }
    }
}
