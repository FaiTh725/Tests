using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup
{
    public class GetProfileCreatedGroupHandler :
        IRequestHandler<GetProfileCreatedGroupQuery, PaginationResponse<GroupInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileCreatedGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<GroupInfo>> Handle(
            GetProfileCreatedGroupQuery request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileEmail, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var allGroups = await unitOfWork.ProfileGroupRepository
                .GetProfileGroupsByCriteria(
                    new GroupsByProfileIdSpecification(
                        profile.Id),
                    cancellationToken);

            var groups = await unitOfWork.ProfileGroupRepository
                .GetProfileGroupsByCriteria(
                    new GroupsByProfileIdPaginationSpecification(
                        profile.Id,
                        request.Page,
                        request.PageSize), 
                    cancellationToken);

            var groupsInfo = groups.Select(groups => new GroupInfo
                {
                    Id = groups.Id,
                    Name = groups.GroupName
                });

            return new PaginationResponse<GroupInfo> 
            { 
                Data = groupsInfo,
                PageSize = request.PageSize,
                Page = request.Page,
                MaxSize = allGroups.Count()
            };

        }
    }
}
