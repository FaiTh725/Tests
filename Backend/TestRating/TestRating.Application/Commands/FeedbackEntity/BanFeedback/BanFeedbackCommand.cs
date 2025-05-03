using MediatR;

namespace TestRating.Application.Commands.FeedbackEntity.BanFeedback
{
    public class BanFeedbackCommand : IRequest
    {
        public long Id { get; set; }
    }
}
