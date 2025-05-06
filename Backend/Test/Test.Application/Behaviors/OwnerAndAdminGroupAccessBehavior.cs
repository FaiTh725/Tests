using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Common.BehaviorsInterfaces;
using Test.Domain.Interfaces;

namespace Test.Application.Behaviors
{
    public class OwnerAndAdminGroupAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IOwnerAndAdminGroupAccess
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public OwnerAndAdminGroupAccessBehavior(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(request.GroupId, cancellationToken);

            if(group is null)
            {
                throw new BadRequestException("Group doesnt exist");
            }

            if (request.Role != "Admin" &&
                group.OwnerId != request.OwnerId)
            {
                throw new ForbiddenAccessException("Only the owner or an admin have access to the group");
            }

            return await next(cancellationToken);
        }
    }
}
