using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Intrefaces;

namespace Test.Application.Commands.Test.UpdateTest
{
    public class UpdateTestHandler :
        IRequestHandler<UpdateTestCommand, long>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public UpdateTestHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(
            UpdateTestCommand request, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);

            if(test is null)
            {
                throw new BadRequestException("Test doesnt exist");
            }

            var isValidUpdate = test.Update(
                request.Name,
                request.Description,
                request.IsPublic,
                request.TestType,
                request.DurationInMinutes);

            if(isValidUpdate.IsFailure)
            {
                throw new BadRequestException("Invalid values to update - " +
                    $"{isValidUpdate.Error}");
            }

            await unitOfWork.TestRepository
                .UpdateTest(test.Id, test, cancellationToken);

            return test.Id;
        }
    }
}
