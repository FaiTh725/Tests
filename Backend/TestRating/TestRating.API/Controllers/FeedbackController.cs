using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestRating.API.Contracts.Feedback;
using TestRating.Application.Commands.FeedbackEntity.ChangeFeedback;
using TestRating.Application.Commands.FeedbackEntity.DeleteFeedback;
using TestRating.Application.Commands.FeedbackEntity.SendFeedback;
using TestRating.Application.Commands.FeedbackReviewEntity.SendFeedbackReview;
using TestRating.Application.Common.Interfaces;
using TestRating.Application.Contacts.File;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbackWithOwner;

namespace TestRating.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ITokenService<ProfileToken> tokenService;

        public FeedbackController(
            IMediator mediator,
            ITokenService<ProfileToken> tokenService)
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTestFeedbacks(
            [FromQuery]GetFeedbacksByTestIdQuery request, CancellationToken cancellationToken)
        {
            var feedbacks = await mediator
                .Send(request, cancellationToken);
        
            return Ok(feedbacks);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> SendFeedback(
            CreateFeedbackRequest request,
            CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await tokenService
                .VerifyToken(token, cancellationToken);

            var feedbackId = await mediator.Send(new SendFeedbackCommand
            {
                TestId = request.TestId,
                Rating = request.Rating,
                Text = request.Text,
                ProfileId = profile.Id,
                FeedbackImages = request.Images is null ? 
                    new List<FileModel>() :
                    request.Images.Select(x => new FileModel 
                    { 
                        Stream = x.OpenReadStream(),
                        Name = x.Name,
                        ContentType = x.ContentType
                    })
                    .ToList()

            }, cancellationToken);

            var feedback = await mediator.Send(new GetFeedbackWithOwnerQuery 
            { 
                Id = feedbackId
            }, cancellationToken);

            return Ok(feedback);
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteFeedback(
            DeleteFeedbackRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await tokenService
                .VerifyToken(token, cancellationToken);

            await mediator.Send(new DeleteFeedbackCommand
            {
                FeedbackId = request.FeedbackId,
                ProfileId = profile.Id,
                ProfileRole = profile.Role
            }, cancellationToken);

            return NoContent();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public async Task<IActionResult> ChangeFeedback(
            ChangeFeedbackRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await tokenService
                .VerifyToken(token, cancellationToken);

            await mediator.Send(new ChangeFeedbackCommand
            {
                FeedbackId = request.FeedbackId,
                Rating = request.Rating,
                Text = request.Text,
                ProfileId = profile.Id,
                ProfileRole = profile.Role,
                NewImages = request.NewImages is null ? 
                    new List<FileModel>() :
                    request.NewImages
                    .Select(x => new FileModel 
                    { 
                        Stream = x.OpenReadStream(), 
                        ContentType = x.ContentType, 
                        Name = x.Name
                    })
                .ToList()

            }, cancellationToken);

            return NoContent();
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> SendReview(
            SendReviewOnFeedbackRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await tokenService
                .VerifyToken(token, cancellationToken);

            await mediator.Send(new SendFeedbackReviewCommand
            {
                FeedbackId  = request.FeedbackId,
                IsPositive = request.IsPositive,
                ProfileId = profile.Id
            }, cancellationToken);

            return Ok();
        }
    }
}
