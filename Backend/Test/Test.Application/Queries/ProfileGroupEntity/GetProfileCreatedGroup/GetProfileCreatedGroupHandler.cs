using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileEntity.Specifications;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup
{
    public class GetProfileCreatedGroupHandler :
        IRequestHandler<GetProfileCreatedGroupQuery, PaginationResponse<GroupWithMembers>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileCreatedGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<GroupWithMembers>> Handle(
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

            var profilesId = groups
                .SelectMany(x => x.MembersId)
                .Distinct()
                .ToList();

            var uniquesProfilesInGroups = await unitOfWork.ProfileRepository
                .GetProfilesByCriteria(
                    new GetProfilesByIdListSpecification(profilesId),
                cancellationToken);

            var profilesDictionary = uniquesProfilesInGroups
                .ToDictionary(
                    x => x.Id, 
                    x => new ProfileResponse 
                    { 
                        Id = x.Id,
                        Email = x.Email,
                        Name = x.Name,   
                    });


            var createdGroups = groups.Select(group => new GroupWithMembers
            {
                Id = group.Id,
                Name = group.GroupName,
                Members = group.MembersId
                    .Where(profilesDictionary.ContainsKey)
                    .Select(id => profilesDictionary[id])
            });

            return new PaginationResponse<GroupWithMembers> 
            { 
                Data = createdGroups,
                PageSize = request.PageSize,
                Page = request.Page,
                MaxSize = allGroups.Count()
            };

        }
    }
}
