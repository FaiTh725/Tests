using Application.Shared.Exceptions;
using MediatR;
using TestRating.Domain.Interfaces;

namespace TestRating.Application.Commands.FeedbackEntity.BanFeedback
{
    public class BanFeedbackHandler :
        IRequestHandler<BanFeedbackCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public BanFeedbackHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(
            BanFeedbackCommand request, 
            CancellationToken cancellationToken)
        {
            var feedback = await unitOfWork.FeedbackRepository
                .GetFeedbackById(request.Id, cancellationToken);

            if(feedback is null)
            {
                throw new BadRequestException("Feedback doesnt exist");
            }

            await unitOfWork.FeedbackRepository
                .SoftDeleteFeedback(request.Id, cancellationToken);
        
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
