using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileEntity;
using Test.Domain.Intrefaces;

namespace Test.Application.Queries.ProfileEntity.GetProfileByEmail
{
    public class GetProfileByEmailHandler :
        IRequestHandler<GetProfileByEmailQuery, ProfileResponse>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileByEmailHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ProfileResponse> Handle(
            GetProfileByEmailQuery request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.Email, cancellationToken);

            if (profile is null)
            {
                throw new NotFoundException("Profile doesnt exist");
            }

            return new ProfileResponse
            {
                Id = profile.Id,
                Email = profile.Email,
                Name = profile.Name
            };
        }
    }
}
