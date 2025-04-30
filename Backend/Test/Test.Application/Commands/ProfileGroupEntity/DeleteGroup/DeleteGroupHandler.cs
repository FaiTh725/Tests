using Application.Shared.Exceptions;
using MediatR;
using Test.Domain.Interfaces;

namespace Test.Application.Commands.ProfileGroupEntity.DeleteGroup
{
    public class DeleteGroupHandler :
        IRequestHandler<DeleteGroupCommand>
    {
        private readonly INoSQLUnitOfWork unitOfWork;

        public DeleteGroupHandler(
            INoSQLUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteGroupCommand request, 
            CancellationToken cancellationToken)
        {
            var group = await unitOfWork.ProfileGroupRepository
                .GetProfileGroup(request.GroupId, cancellationToken);

            if(group is null)
            {
                throw new BadRequestException("Group doesnt exist");
            }

            await unitOfWork.ProfileGroupRepository
                .DeleteGroup(request.GroupId, cancellationToken);
        }
    }
}
