using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Domain.Intrefaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileJoinedGroup
{
    public class GetProfileJoinedGroupHandler :
        IRequestHandler<GetProfileJoinedGroupQuery, IEnumerable<GroupInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileJoinedGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GroupInfo>> Handle(
            GetProfileJoinedGroupQuery request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileId, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var groups = await unitOfWork.ProfileGroupRepository
                .GetProfileGroupsByCriteria(
                    new GroupsProfileJoinedSpecification(profile.Id), 
                    cancellationToken);


            return groups.Select(x => new GroupInfo
            {
                Id = x.Id,
                Name = x.GroupName
            });
        }
    }
}
