using MediatR;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.Specifications;
using Test.Domain.Interfaces;

namespace Test.Application.Queries.ProfileEntity.GetProfilesByEmail
{
    public class GetProfilesByEmailHandler :
        IRequestHandler<GetProfilesByEmailQuery, IEnumerable<ProfileResponse>>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfilesByEmailHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProfileResponse>> Handle(
            GetProfilesByEmailQuery request, 
            CancellationToken cancellationToken)
        {
            var profiles = await unitOfWork.ProfileRepository
                .GetProfilesByCriteria(
                    new GetProfilesByEmailFirstLettersSpecification(request.Email),
                    cancellationToken);

            return profiles.Select(x => new ProfileResponse 
            { 
                Email = x.Email,
                Id = x.Id,
                Name = x.Name
            });
        }
    }
}
