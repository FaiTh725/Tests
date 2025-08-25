using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.Common;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Contracts.Test;
using Test.Application.Queries.ProfileEntity.Specifications;
using Test.Application.Queries.Test.Specifications;
using Test.Application.Queries.TestAccessEntity.Specifications;
using Test.Domain.Enums;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupTests
{
    public class GetGroupTestsHandler :
        IRequestHandler<GetGroupTestsQuery, PaginationResponse<TestInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetGroupTestsHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PaginationResponse<TestInfo>> Handle(
            GetGroupTestsQuery request, 
            CancellationToken cancellationToken)
        {
            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(request.GroupId, cancellationToken);

            if(group is null)
            {
                throw new BadRequestException("Group Doesnt exist");
            }

            var paginatedAccesses = await unitOfWork.AccessRepository
                .GetPaginatedAccessesByCriteria(
                new GetAccessesPaginatedByTargetEntityIdAndTypeSpecification(
                    request.GroupId, TargetAccessEntityType.Group,
                    request.Page, request.PageSize),
                cancellationToken);

            var testsId = paginatedAccesses.Items
                .Select(x => x.TestId)
                .ToList();

            var tests = await unitOfWork.TestRepository.GetTestsByCriteria(
                new GetTestsByIdListSpecification(testsId), 
                cancellationToken);

            var ownersId = tests
                .Select(x => x.ProfileId)
                .Distinct()
                .ToList();

            var owners = await unitOfWork.ProfileRepository
                .GetProfilesByCriteria(new GetProfilesByIdListSpecification(ownersId), 
                cancellationToken);

            var ownersDictinary = owners.ToDictionary(
                x => x.Id, 
                x => new ProfileResponse 
                { 
                    Name = x.Name, 
                    Email = x.Email, 
                    Id = x.Id
                });


            return new PaginationResponse<TestInfo>
            {
                PageSize = request.PageSize,
                Page = request.Page,
                MaxSize = paginatedAccesses.TotalCount,
                Data = tests.Select(x => new TestInfo 
                { 
                    Owner = ownersDictinary[x.ProfileId],
                    Id = x.Id,
                    CreatedTime = x.CreatedTime,
                    Description = x.Description,
                    DurationInMinutes = x.DurationInMinutes,
                    IsPublic = x.IsPublic,
                    Name = x.Name,
                    TestType = x.TestType.ToString()
                })
            };
        }
    }
}
