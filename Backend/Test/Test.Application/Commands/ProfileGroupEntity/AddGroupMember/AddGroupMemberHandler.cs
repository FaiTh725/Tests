using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.ProfileGroupEntity.AddGroupMember
{
    public class AddGroupMemberHandler :
        IRequestHandler<AddGroupMemberCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public AddGroupMemberHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            AddGroupMemberCommand request, 
            CancellationToken cancellationToken)
        {
            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(request.GroupId, cancellationToken);

            if(group is null)
            {
                throw new BadRequestException("Group doesnt exist");
            }

            if(group.OwnerId == request.ProfileId ||
                group.MembersId.Any(x => x == request.ProfileId))
            {
                throw new ConflictException("New member already added");
            }

            var newMember = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileId, cancellationToken);

            if (newMember is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            group.AddMember(newMember.Id);
            unitOfWork.TrackEntity(group);
            
            await unitOfWork.ProfileGroupRepository
                .UpdateGroup(group.Id, group, cancellationToken: cancellationToken);
        }
    }
}
