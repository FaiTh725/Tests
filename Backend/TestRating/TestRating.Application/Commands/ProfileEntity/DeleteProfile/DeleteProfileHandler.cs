using Application.Shared.Exceptions;
using MediatR;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.ProfileEntity.DeleteProfile
{
    public class DeleteProfileHandler :
        IRequestHandler<DeleteProfileCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteProfileHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteProfileCommand request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfileById(request.Id, cancellationToken);

            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            await unitOfWork.ProfileRepository
                .DeleteProfile(profile.Id, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
