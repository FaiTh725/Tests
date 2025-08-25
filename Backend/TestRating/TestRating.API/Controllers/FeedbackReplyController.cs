using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestRating.API.Contracts.FeedbackReply;
using TestRating.API.Filters;
using TestRating.Application.Commands.ReplyEntity.DeleteReply;
using TestRating.Application.Commands.ReplyEntity.SendReply;
using TestRating.Application.Commands.ReplyEntity.UpdateReply;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies;
using TestRating.Application.Queries.FeedbackReplyEntity.GetReplyWithOwnerById;

namespace TestRating.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackReplyController : ControllerBase
    {
        private readonly IMediator mediator;

        public FeedbackReplyController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> SendFeedbackReply(
            SendReplyRequest request, CancellationToken cancellationToken)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

            var replyId = await mediator.Send(new SendReplyCommand
            {
                FeedbackId = request.FeedbackId,
                OwnerId = profile.Id,
                Text = request.Text,
            }, cancellationToken);

            var reply = await mediator.Send(new GetReplyWithOwnerByIdQuery
            {
                Id = replyId
            }, cancellationToken);

            return Ok(reply);
        }

        [HttpDelete("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> DeleteFeedbackReply(
            DeleteReplyRequest request, CancellationToken cancellationToken)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

            await mediator.Send(new DeleteReplyCommand
            {
                ProfileId = profile.Id,
                ProfileRole = profile.Role,
                ReplyId = request.ReplyId
            }, cancellationToken);

            return NoContent();
        }

        [HttpPatch("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> ChangeReply(
            ChangeReplyRequest request, CancellationToken cancellationToken)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

            await mediator.Send(new UpdateReplyCommand
            {
                ProfileId = profile.Id,
                ProfileRole = profile.Role,
                ReplyId= request.ReplyId,
                Text = request.Text
            }, cancellationToken);

            return NoContent();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetFeedbackReplies(
            [FromQuery] GetFeedbackRepliesQuery request, CancellationToken cancellationToken)
        {
            var replies = await mediator
                .Send(request, cancellationToken);

            return Ok(replies);
        }
    }
}
