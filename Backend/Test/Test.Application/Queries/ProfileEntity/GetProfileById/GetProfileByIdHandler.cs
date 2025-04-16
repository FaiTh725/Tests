using Application.Shared.Exceptions;
using MediatR;
using Test.Application.Contracts.ProfileEntity;
using Test.Domain.Intrefaces;

namespace Test.Application.Queries.ProfileEntity.GetProfileById
{
    public class GetProfileByIdHandler :
        IRequestHandler<GetProfileByIdQuery, ProfileResponse>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GetProfileByIdHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<ProfileResponse> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.Id, cancellationToken);
        
            if(profile is null)
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
