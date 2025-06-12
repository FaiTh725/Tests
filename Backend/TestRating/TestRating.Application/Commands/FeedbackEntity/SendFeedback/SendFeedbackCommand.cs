using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;
using TestRating.Application.Contacts.File;

namespace TestRating.Application.Commands.FeedbackEntity.SendFeedback
{
    public class SendFeedbackCommand : 
        IRequest<long>,
        ICheckTestIsExist
    {
        public List<FileModel> FeedbackImages { get; set; } = new List<FileModel>();

        public string Text { get; set; } = string.Empty;

        public long TestId { get; set; }

        public long ProfileId { get; set; }

        public int Rating { get; set; }
    }
}
