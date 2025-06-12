using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Contacts.Profile;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Queries.ProfileEntity.GetProfileById
{
    public class GetProfileByIdHandler :
        IRequestHandler<GetProfileByIdQuery, BaseProfileResponse>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetProfileByIdHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<BaseProfileResponse> Handle(
            GetProfileByIdQuery request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfileById(request.Id, cancellationToken);

            if(profile is null)
            {
                throw new NotFoundException("Profile doesnt exist");
            }

            return new BaseProfileResponse
            {
                Id = profile.Id,
                Email = profile.Email,   
                Name = profile.Name,
            };
        }
    }
}
