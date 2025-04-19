using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.BehaviorsIntrfaces;
using Test.Domain.Intrefaces;

namespace Test.Application.Behaviors
{
    public class OwnerAndAdminTestAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IOwnerAndAdminTestAccess
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public OwnerAndAdminTestAccessBehavior(
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

            if (test is null)
            {
                throw new NotFoundException("Test doesnt exist");
            }

            var profile = await unitOfWork.ProfileRepository
                .GetProfile(test.ProfileId, cancellationToken);

            if(profile is null ||
                request.Role != "Admin")
            {
                throw new ForbiddenAccessException("Only a owner and admin have access to the test");
            }

            return await next(cancellationToken);
        }
    }
}
