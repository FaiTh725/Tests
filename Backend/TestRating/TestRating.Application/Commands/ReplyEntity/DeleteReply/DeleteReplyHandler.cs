using Application.Shared.Exceptions;
using MediatR;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.ReplyEntity.DeleteReply
{
    public class DeleteReplyHandler :
        IRequestHandler<DeleteReplyCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteReplyHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            DeleteReplyCommand request, 
            CancellationToken cancellationToken)
        {
            var reply = await unitOfWork.ReplyRepository
                .GetReply(request.ReplyId, cancellationToken);

            if(reply is null)
            {
                throw new BadRequestException("Reply doesnt exist");
            }

            await unitOfWork.ReplyRepository
                .HardDeleteReply(reply.Id, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
