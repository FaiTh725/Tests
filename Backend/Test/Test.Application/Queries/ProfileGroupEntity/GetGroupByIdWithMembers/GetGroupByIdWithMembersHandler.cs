using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupByIdWithMembers
{
    public class GetGroupByIdWithMembersHandler :
        IRequestHandler<GetGroupByIdWithMembersQuery, GroupWithMembers>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetGroupByIdWithMembersHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<GroupWithMembers> Handle(
            GetGroupByIdWithMembersQuery request, 
            CancellationToken cancellationToken)
        {
            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(request.GroupId, cancellationToken);

            if(group is null)
            {
                throw new NotFoundException("Group doesnt exist");
            }

            var groupMembers = await unitOfWork.ProfileRepository
                .GetProfilesByCriteria(
                    new GetProfilesByIdListSpecification(group.MembersId), 
                    cancellationToken);

            return new GroupWithMembers
            {
                Id = group.Id,
                Name = group.GroupName,
                Members = groupMembers
                    .Select(x => 
                    new ProfileResponse 
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Name = x.Name 
                    })

            };
        }
    }
}
