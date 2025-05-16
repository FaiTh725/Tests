using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;
using TestRating.Application.Queries.FeedbackReplyEntity.Specifications;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Behaviors
{
    public class OwnerAndAdminReplyAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IOwnerAndAdminReplyAccess
    {
        private readonly IUnitOfWork unitOfWork;

        public OwnerAndAdminReplyAccessBehavior(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var reply = await unitOfWork.ReplyRepository
                .GetReply(request.ReplyId, cancellationToken);
        
            if(reply is null)
            {
                throw new BadRequestException("Feedback Reply doesnt exist");
            }

            if(reply.OwnerId != request.ProfileId &&
                request.ProfileRole != "Admin")
            {
                throw new ForbiddenAccessException("Only the owner and an admin have access to the feedback");
            }

            return await next(cancellationToken);
        }
    }
}
