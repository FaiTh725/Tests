using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;

namespace TestRating.Application.Commands.ReplyEntity.UpdateReply
{
    public class UpdateReplyCommand : 
        IRequest,
        IOwnerAndAdminReplyAccess
    {
        public long ReplyId { get; set; }

        public string Text { get; set; } = string.Empty;
        
        public long ProfileId { get; set; }

        public string ProfileRole { get; set; } = string.Empty;
    }
}
