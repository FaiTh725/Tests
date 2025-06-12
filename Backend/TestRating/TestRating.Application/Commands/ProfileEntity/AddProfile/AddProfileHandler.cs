using Application.Shared.Exceptions;
using MediatR;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.ProfileEntity.AddProfile
{
    public class AddProfileHandler :
        IRequestHandler<AddProfileCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;

        public AddProfileHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(
            AddProfileCommand request, 
            CancellationToken cancellationToken)
        {
            var existedProfile = await unitOfWork.ProfileRepository
                .GetProfileByEmail(request.Email, cancellationToken);

            if(existedProfile is not null)
            {
                throw new ConflictException($"Profile with email {request.Email} " +
                    "already registered");
            }

            var profileEntity = Profile.Initialize(
                request.Email,
                request.Name);

            if(profileEntity.IsFailure)
            {
                throw new BadRequestException("Invalid request " +
                    profileEntity.Error);
            }

            var dbProfile = await unitOfWork.ProfileRepository
                .AddProfile(profileEntity.Value, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return dbProfile.Id;
        }
    }
}
