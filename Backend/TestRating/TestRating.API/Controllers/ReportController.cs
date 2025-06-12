using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestRating.API.Contracts.FeedbackReport;
using TestRating.API.Filters;
using TestRating.Application.Commands.FeedbackReportEntity.ReviewReport;
using TestRating.Application.Commands.FeedbackReportEntity.SendReport;
using TestRating.Application.Contacts.Profile;
using TestRating.Application.Queries.FeedbackReportEntity.GetReportWithReviewer;

namespace TestRating.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReportController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "AdminOnly")]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> SendReport(
            SendReportRequest request, 
            CancellationToken cancellationToken = default)
        {
            var profile = (DecodedProfile)HttpContext.Items["profile"]!;

            var reportId = await mediator.Send(new SendReportCommand 
            { 
                Message = request.Message,
                ReportedFeedbackId = request.ReportedFeedbackId,
                ReviewerId = profile.Id
            }, cancellationToken);

            var report = await mediator.Send(
                new GetReportWithReviewerQuery 
                { 
                    Id = reportId
                }, cancellationToken);

            return Ok(report);
        }


        [HttpPatch("[action]")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ReviewReport(
            ReviewReportCommand request, CancellationToken cancellationToken)
        {
            await mediator.Send(request, cancellationToken);

            return NoContent();
        }
    }
}
