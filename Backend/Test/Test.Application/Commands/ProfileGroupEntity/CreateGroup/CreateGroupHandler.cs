using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Entities;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.ProfileGroupEntity.CreateGroup
{
    public class CreateGroupHandler :
        IRequestHandler<CreateGroupCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public CreateGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(
            CreateGroupCommand request, 
            CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.OwnerId, cancellationToken);

            if (profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var groupEntity = ProfileGroup.Initialize(
                request.GroupName, profile.Id);

            if(groupEntity.IsFailure)
            {
                throw new BadRequestException("Invalid request - " + groupEntity.Error);
            }

            var newProfileGroup = await unitOfWork.ProfileGroupRepository
                .AddGroup(groupEntity.Value, cancellationToken: cancellationToken);

            return newProfileGroup.Id;
        }
    }
}
