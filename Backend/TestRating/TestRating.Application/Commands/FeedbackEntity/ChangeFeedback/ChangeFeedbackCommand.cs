using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;
using TestRating.Application.Contacts.File;

namespace TestRating.Application.Commands.FeedbackEntity.ChangeFeedback
{
    public class ChangeFeedbackCommand : 
        IRequest<long>,
        IOwnerAndAdminFeedbackAccess
    {
        public long FeedbackId { get; set; }

        public List<FileModel> NewImages { get; set; } = new List<FileModel>();

        public string Text { get; set; } = string.Empty;

        public int Rating { get; set; }

        public long ProfileId { get; set; }

        public string ProfileRole { get; set; } = string.Empty;
    }
}
