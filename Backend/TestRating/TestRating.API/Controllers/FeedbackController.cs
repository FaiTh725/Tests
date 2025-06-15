using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestRating.API.Contracts.Feedback;
using TestRating.API.Filters;
using TestRating.Application.Commands.FeedbackEntity.ChangeFeedback;
using TestRating.Application.Commands.FeedbackEntity.DeleteFeedback;
using TestRating.Application.Commands.FeedbackEntity.SendFeedback;
using TestRating.Application.Commands.FeedbackReviewEntity.SendFeedbackReview;
using TestRating.Application.Contacts.File;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestId;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbacksByTestIdAndRating;
using TestRating.Application.Queries.FeedbackEntity.GetFeedbackWithOwner;
using TestRating.Application.Queries.FeedbackEntity.GetTestStatisticsByFeedbacks;

namespace TestRating.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IMediator mediator;

        public FeedbackController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetTestFeedbacks(
            [FromQuery]GetFeedbacksByTestIdQuery request, CancellationToken cancellationToken)
        {
            var feedbacks = await mediator
                .Send(request, cancellationToken);
        
            return Ok(feedbacks);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetFeedback(
            long feedbackId, 
            CancellationToken cancellationToken)
        {
            var feedback = await mediator.Send(new GetFeedbackWithOwnerQuery 
            { 
                Id = feedbackId
            }, cancellationToken);

            return Ok(feedback);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetTestStatistics(
            long testId, CancellationToken cancellationToken)
        {
            var testStatistics = await mediator.Send(new GetTestStatisticsByFeedbacksQuery
            {
                TestId = testId
            }, cancellationToken);

            return Ok(testStatistics);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetFeebacksByFilter(
            [FromQuery]GetFeedbacksByTestIdAndRatingQuery query, CancellationToken cancellationToken)
        {
            var feedbacks = await mediator.Send(query, cancellationToken);

            return Ok(feedbacks);
        }

        [HttpPost("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> SendFeedback(
            CreateFeedbackRequest request,
            CancellationToken cancellationToken)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

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
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> DeleteFeedback(
            DeleteFeedbackRequest request, CancellationToken cancellationToken)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

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
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> ChangeFeedback(
            ChangeFeedbackRequest request, CancellationToken cancellationToken)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

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
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> SendReview(
            SendReviewOnFeedbackRequest request, CancellationToken cancellationToken)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

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
