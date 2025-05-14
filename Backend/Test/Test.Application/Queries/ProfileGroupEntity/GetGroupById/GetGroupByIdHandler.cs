using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupById
{
    public class GetGroupByIdHandler :
        IRequestHandler<GetGroupByIdQuery, GroupInfoWithOwner>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetGroupByIdHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<GroupInfoWithOwner> Handle(
            GetGroupByIdQuery request, 
            CancellationToken cancellationToken)
        {
            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(request.Id, cancellationToken);

            if(group is null)
            {
                throw new NotFoundException("Profile group doesnt exist");
            }

            var groupOwner = await unitOfWork.ProfileRepository
                .GetProfile(group.OwnerId, cancellationToken);

            if(groupOwner is null)
            {
                throw new InternalServerErrorException("Group doesnt have an owner");
            }

            return new GroupInfoWithOwner
            {
                Id = group.Id,
                Name = group.GroupName,
                Owner = new ProfileResponse
                {
                    Id = groupOwner.Id,
                    Email = groupOwner.Email,
                    Name = groupOwner.Name
                }
            };
        }
    }
}
