using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.Test.DeleteTest
{
    public class DeleteTestHandler :
        IRequestHandler<DeleteTestCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public DeleteTestHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteTestCommand request, 
            CancellationToken cancellationToken)
        {
            var test = await unitOfWork.TestRepository
                .GetTest(request.TestId, cancellationToken);

            if(test is null)
            {
                throw new NotFoundException("Test doesnt exist");
            }

            await unitOfWork.TestRepository
                .DeleteTest(request.TestId, cancellationToken);

            test.Delete();
            unitOfWork.TrackEntity(test);
        }
    }
}
