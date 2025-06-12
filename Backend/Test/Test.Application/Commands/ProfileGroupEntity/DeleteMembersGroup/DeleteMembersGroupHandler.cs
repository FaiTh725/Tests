using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.ProfileGroupEntity.DeleteMembersGroup
{
    public class DeleteMembersGroupHandler :
        IRequestHandler<DeleteMembersGroupCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public DeleteMembersGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteMembersGroupCommand request, 
            CancellationToken cancellationToken)
        {
            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(request.GroupId, cancellationToken);

            if(group is null)
            {
                throw new BadRequestException("Group doesnt exist");
            }

            var deleteMembersResult = group.DeleteMembers(request.MembersId);

            if(deleteMembersResult.IsFailure)
            {
                throw new BadRequestException(deleteMembersResult.Error);
            }

            await unitOfWork.ProfileGroupRepository
                .UpdateGroup(group.Id, group, cancellationToken: cancellationToken);
        }
    }
}
