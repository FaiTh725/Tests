using Application.Shared.Exceptions;
using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Behaviors
{
    public class OwnerAndAdminFeedbackAccessBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IOwnerAndAdminFeedbackAccess
    {
        private readonly IUnitOfWork unitOfWork;

        public OwnerAndAdminFeedbackAccessBehavior(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackById(request.FeedbackId, cancellationToken);

            if(feedback is null)
            {
                throw new BadRequestException("Feedback doesnt exist");
            }

            if(request.ProfileRole != "Admin" &&
                feedback.Id != request.ProfileId)
            {
                throw new ForbiddenAccessException("Only the owner and an admin have access to the feedback");
            }

            return await next(cancellationToken);
        }
    }
}
