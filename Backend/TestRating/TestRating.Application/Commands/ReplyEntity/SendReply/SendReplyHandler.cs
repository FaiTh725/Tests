using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Entities;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.ReplyEntity.SendReply
{
    public class SendReplyHandler :
        IRequestHandler<SendReplyCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;

        public SendReplyHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(
            SendReplyCommand request, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackById(request.FeedbackId, cancellationToken);

            if(feedback is null)
            {
                throw new BadRequestException("Feedback doesnt exist");
            }

            var replyFromProfile = await unitOfWork.ReplyRepository
                .GetReplyByCriteria(new ReplyByOwnerAndFeedbackIdSpecification(
                    request.OwnerId, request.FeedbackId),
                    cancellationToken);

            if(replyFromProfile is not null)
            {
                throw new ConflictException("Current profile has already sent a reply on this feedback");
            }

            var replyEntity = FeedbackReply.Initialize(
                request.Text,
                request.FeedbackId,
                request.OwnerId);

            if(replyEntity.IsFailure)
            {
                throw new BadRequestException("invalid request field - " +
                    replyEntity.Error);
            }

            var replyDb = await unitOfWork.ReplyRepository
                .AddReply(replyEntity.Value, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return replyDb.Id;
        }
    }
}
