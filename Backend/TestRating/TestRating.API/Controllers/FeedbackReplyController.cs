using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestRating.API.Contracts.FeedbackReply;
using TestRating.Application.Commands.ReplyEntity.DeleteReply;
using TestRating.Application.Commands.ReplyEntity.SendReply;
using TestRating.Application.Commands.ReplyEntity.UpdateReply;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReplyEntity.GetFeedbackReplies;
using TestRating.Application.Queries.FeedbackReplyEntity.GetReplyWithOwnerById;

namespace TestRating.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedbackReplyController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ITokenService<ProfileToken> tokenService;

        public FeedbackReplyController(
            IMediator mediator,
            ITokenService<ProfileToken> tokenService)
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> SendFeedbackReply(
            SendReplyRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await tokenService
                .VerifyToken(token, cancellationToken);

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
        public async Task<IActionResult> DeleteFeedbackReply(
            DeleteReplyRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await tokenService
                .VerifyToken(token, cancellationToken);

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
        public async Task<IActionResult> ChangeReply(
            ChangeReplyRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await tokenService
                .VerifyToken(token, cancellationToken);

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
            [FromQuery]GetFeedbackRepliesQuery request, CancellationToken cancellationToken)
        {
            var replies = await mediator.Send(request, cancellationToken);

            return Ok(replies);
        }
    }
}
