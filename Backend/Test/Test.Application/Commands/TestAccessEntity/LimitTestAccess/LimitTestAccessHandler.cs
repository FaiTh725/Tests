using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.TestAccessEntity.LimitTestAccess
{
    public class LimitTestAccessHandler :
        IRequestHandler<LimitTestAccessCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public LimitTestAccessHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            LimitTestAccessCommand request, 
            CancellationToken cancellationToken)
        {
            var testAccess = await unitOfWork.AccessRepository
                .GetTestAccess(
                request.TestId, 
                request.TargetEntityId, 
                request.TargetEntity,
                cancellationToken);

            if (testAccess is null)
            {
                throw new BadRequestException("Access doesnt exist");
            }

            await unitOfWork.AccessRepository
                .DeleteTestAccess(testAccess.Id, cancellationToken: cancellationToken);
        }
    }
}
