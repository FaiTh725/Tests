using MediatR;
using Test.Application.Contracts.ProfileGroupEntity;
using Test.Application.Queries.ProfileGroupEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileGroupEntity.GetGroupsByFirstLettersName
{
    public class GetGroupsByFirstLettersNameHandler :
        IRequestHandler<GetGroupsByFirstLettersNameQuery, IEnumerable<GroupInfo>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetGroupsByFirstLettersNameHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GroupInfo>> Handle(
            GetGroupsByFirstLettersNameQuery request, 
            CancellationToken cancellationToken)
        {
            var groups = await unitOfWork.ProfileGroupRepository
                .GetProfileGroupsByCriteria(
                new GroupsByFirstLetterNameSpecification(request.GroupName), 
                cancellationToken);

            return groups
                .Select(x => new GroupInfo 
                { 
                    Id = x.Id, 
                    Name = x.GroupName
                });
        }
    }
}
