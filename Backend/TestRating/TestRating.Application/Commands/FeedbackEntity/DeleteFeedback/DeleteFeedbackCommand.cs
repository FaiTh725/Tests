using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;

namespace TestRating.Application.Commands.FeedbackEntity.DeleteFeedback
{
    public class DeleteFeedbackCommand : 
        IRequest,
        IOwnerAndAdminFeedbackAccess
    {
        public long FeedbackId { get; set; }

        public long ProfileId { get; set; }

        public string ProfileRole { get; set; } = string.Empty;
    }
}
