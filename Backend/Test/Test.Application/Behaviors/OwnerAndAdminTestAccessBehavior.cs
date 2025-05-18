using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Domain.Interfaces;

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
                throw new BadRequestException("Test doesnt exist");
            }

            if (request.Role != "Admin" &&
                test.ProfileId != request.OwnerId)
            {
                throw new ForbiddenAccessException("Only the owner or an admin have access to the test");
            }

            return await next(cancellationToken);
        }
    }
}
