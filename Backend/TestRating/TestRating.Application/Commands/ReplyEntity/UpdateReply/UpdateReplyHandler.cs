using Application.Shared.Exceptions;
using MediatR;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.ReplyEntity.UpdateReply
{
    public class UpdateReplyHandler :
        IRequestHandler<UpdateReplyCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public UpdateReplyHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            UpdateReplyCommand request, 
            CancellationToken cancellationToken)
        {
            var reply = await unitOfWork.ReplyRepository
                .GetReply(request.ReplyId, cancellationToken);

            if(reply is null)
            {
                throw new BadRequestException("Reply doesnt exist");
            }

            var isValidChanges = reply
                .ChangeReply(request.Text);

            if(isValidChanges.IsFailure)
            {
                throw new BadRequestException("Invalid value to update. " +
                    isValidChanges.Error);
            }

            await unitOfWork.ReplyRepository
                .UpdateReply(reply.Id, reply, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
