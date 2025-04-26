using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Domain.Intrefaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup
{
    public class GetProfileCreatedGroupHandler :
        IRequestHandler<GetProfileCreatedGroupQuery, IEnumerable<GroupInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileCreatedGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GroupInfo>> Handle(
            GetProfileCreatedGroupQuery request, 
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
                    new GroupsByProfileIdSpecification(profile.Id), 
                    cancellationToken);

            return groups.Select(groups => new GroupInfo
            {
                Id = groups.Id,
                Name = groups.GroupName
            });
        }
    }
}
