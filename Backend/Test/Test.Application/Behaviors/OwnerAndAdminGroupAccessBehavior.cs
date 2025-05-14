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

            var owner = await unitOfWork.ProfileRepository
                .GetProfile(request.OwnerEmail, cancellationToken);

            if (owner is null)
            {
                throw new InternalServerErrorException("Unexpected error, group doesnt have an owner");
            }

            if (request.Role != "Admin" &&
                group.OwnerId != owner.Id)
            {
                throw new ForbiddenAccessException("Only the owner or an admin have access to the group");
            }

            return await next(cancellationToken);
        }
    }
}
