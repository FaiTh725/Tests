using MediatR;

namespace TestRating.Application.Commands.ReplyEntity.SendReply
{
    public class SendReplyCommand : 
        IRequest<long>
    {
        public long FeedbackId { get; set; }

        public string Text { get; set; } = string.Empty;

        public long OwnerId { get; set; }
    }
}
