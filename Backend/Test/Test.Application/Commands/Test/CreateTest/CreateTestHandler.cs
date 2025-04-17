using Application.Shared.Exceptions;
using MediatR;
using TestEntity = Test.Domain.Entities.Test;
using Test.Domain.Intrefaces;

namespace Test.Application.Commands.Test.CreateTest
{
    public class CreateTestHandler :
        IRequestHandler<CreateTestCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public CreateTestHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateTestCommand request, CancellationToken cancellationToken)
        {
            var profile = await unitOfWork.ProfileRepository
                .GetProfile(request.ProfileId, cancellationToken);
        
            if(profile is null)
            {
                throw new BadRequestException("Profile doesnt exist");
            }

            var testEntity = TestEntity.Initialize(
                request.Name, request.Description, 
                profile.Id, request.TestType, request.IsPublic);
    
            if(testEntity.IsFailure)
            {
                throw new BadRequestException("Incorrect request - " + testEntity.Error);
            }

            var newTest = await unitOfWork.TestRepository
                .AddTest(testEntity.Value, cancellationToken);

            return newTest.Id;
        }
    }
}
