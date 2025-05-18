using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Entities;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.ProfileEntity.CreateProfile
{
    public class CreateProfileHandler :
        IRequestHandler<CreateProfileCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public CreateProfileHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(
            CreateProfileCommand request, 
            CancellationToken cancellationToken)
        {
            var existProfile = await unitOfWork.ProfileRepository
                .GetProfile(request.Email, cancellationToken);

            if(existProfile is not null)
            {
                throw new ConflictException("Email " + request.Email + 
                    " has already registered");
            }

            var profileEntity = Profile.Initialize(
                request.Name,
                request.Email);

            if(profileEntity.IsFailure)
            {
                throw new BadRequestException("Incorrect request - " + profileEntity.Error);
            }

            var newProfie = await unitOfWork.ProfileRepository
                .AddProfile(profileEntity.Value, cancellationToken);

            return newProfie.Id;
        }
    }
}
