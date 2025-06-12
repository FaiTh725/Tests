using MediatR;
using TestRating.Application.Common.BehaviorInterfaces;

namespace TestRating.Application.Commands.ReplyEntity.DeleteReply
{
    public class DeleteReplyCommand : 
        IRequest,
        IOwnerAndAdminReplyAccess
    {
        public long ReplyId { get; set; }

        public long ProfileId { get; set; }
        
        public string ProfileRole { get; set; } = string.Empty;
    }
}
