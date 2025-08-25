using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileJoinedGroup
{
    public class GetProfileJoinedGroupHandler :
        IRequestHandler<GetProfileJoinedGroupQuery, PaginationResponse<GroupInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileJoinedGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<GroupInfo>> Handle(
            GetProfileJoinedGroupQuery request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileEmail, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var groups = await unitOfWork.ProfileGroupRepository
                .GetPaginatedProfileGroupsByCriteria(
                    new GroupsProfileJoinedPaginationSpecification(
                        profile.Id, 
                        request.Page,
                        request.PageSize), 
                    cancellationToken);

            return new PaginationResponse<GroupInfo> 
            { 
                Data = groups.Items.Select(x => new GroupInfo
                {
                    Id = x.Id,
                    Name = x.GroupName
                }),
                MaxSize = groups.TotalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
