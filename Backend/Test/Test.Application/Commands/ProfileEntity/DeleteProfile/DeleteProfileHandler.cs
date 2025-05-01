using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.ProfileEntity.DeleteProfile
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
                .GetProfile(request.ProfileId, cancellationToken);

            if (profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            await unitOfWork.ProfileRepository
                .DeleteProfile(request.ProfileId, cancellationToken);
        }
    }
}
