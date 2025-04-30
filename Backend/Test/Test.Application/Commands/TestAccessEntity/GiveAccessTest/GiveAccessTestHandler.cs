using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Entities;
using Test.Domain.Enums;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.TestAccessEntity.GiveAccessTest
{
    public class GiveAccessTestHandler :
        IRequestHandler<GiveAccessTestCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public GiveAccessTestHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
            
        public async Task<long> Handle(
            GiveAccessTestCommand request, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);

            if(test is null)
            {
                throw new BadRequestException("Test doesnt exist");
            }

            if(test.IsPublic)
            {
                throw new ConflictException("Test has public access");
            }

            var existedTestAccess = await unitOfWork.AccessRepository
                .GetTestAccess(
                request.TestId, 
                request.AccessTargetEntityId, 
                request.TargetEntity,
                cancellationToken);

            if (existedTestAccess is not null)
            {
                throw new ConflictException("Entity already has access");
            }

            long? targetEntityId = null;

            if(request.TargetEntity == TargetAccessEntityType.Group)
            {
                targetEntityId = (await unitOfWork.ProfileGroupRepository
                    .GetProfileGroup(request.AccessTargetEntityId, cancellationToken))?.Id;
            }
            else
            {
                targetEntityId = (await unitOfWork.ProfileRepository
                    .GetProfile(request.AccessTargetEntityId, cancellationToken))?.Id;
            }

            if(targetEntityId is null)
            {
                throw new BadRequestException("Target entity doesnt exist");
            }

            var testAccessEntity = TestAccess.Initialize(
                test.Id,
                targetEntityId.Value,
                request.TargetEntity);

            if(testAccessEntity.IsFailure)
            {
                throw new BadRequestException("Request has invalid value - "
                    +testAccessEntity.Error);
            }

            var testAccess = await unitOfWork.AccessRepository
                .AddTestAccess(testAccessEntity.Value, cancellationToken);

            return testAccess.Id;
        }
    }
}
