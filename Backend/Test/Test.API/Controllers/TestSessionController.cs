using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Common;
using Test.API.Contracts.Test;
using Test.API.Filters;
using Test.Application.Commands.Test.SendTestAnswer;
using Test.Application.Commands.Test.StartTest;
using Test.Application.Commands.Test.StopTest;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.Test.GetTestToPass;
using Test.Application.Queries.TestSessions.GetFinishedSessionById;
using Test.Application.Queries.TestSessions.GetProfileSessionResult;
using Test.Application.Queries.TestSessions.GetProfileSessionsResults;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestSessionController : ControllerBase
    {
        private readonly IMediator mediator;

        public TestSessionController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetSession(
            long sessionId, CancellationToken cancellationToken = default)
        {
            var session = await mediator.Send(new GetFinishedSessionByIdQuery
            {
                Id = sessionId,
            }, 
            cancellationToken);

            return Ok(session);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetSessionResult(
            long sessionId, CancellationToken cancellationToken = default)
        {
            var session = await mediator.Send(new GetProfileSessionResultQuery
            {
                Id = sessionId,
            },
            cancellationToken);

            return Ok(session);
        }

        [HttpGet("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> GetProfileSessions(
            [FromQuery]GetPaginatedDataRequest request, 
            CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            var sessions = await mediator.Send(new GetProfileSessionsResultsQuery
            {
                ProfileId = profile.Id,
                Page = request.Page,
                PageSize = request.PageSize,
            }, 
            cancellationToken);

            return Ok(sessions);
        }

        [HttpPost("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> StartTest(
            StartTestRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            var sessionId = await mediator.Send(new StartTestCommand
            {
                TestId = request.TestId,
                ProfileId = profile.Id
            },
            cancellationToken);

            Response.Cookies.Append("test_session", sessionId.ToString());

            var testToPass = await mediator.Send(new GetTestToPassQuery
            {
                Id = request.TestId
            }, cancellationToken);

            return Ok(testToPass);
        }

        [HttpPost("[action]")]
        [Authorize]
        [ServiceFilter(typeof(SessionRequiredFilter))]
        public async Task<IActionResult> StopTest(
            CancellationToken cancellationToken)
        {
            var sessionId = new Guid(HttpContext.Items["SessionId"]!.ToString()!);

            var testSession = await mediator.Send(new StopTestCommand 
            {
                SessionId = sessionId 
            }, 
            cancellationToken);

            Response.Cookies.Delete("test_session");

            return Ok(testSession);
        }

        [HttpPost("[action]")]
        [Authorize]
        [ServiceFilter(typeof(SessionRequiredFilter))]
        public async Task<IActionResult> SendTestAnswer(
            SendTestAnswerRequest request, CancellationToken cancellationToken)
        {
            var sessionId = new Guid(HttpContext.Items["SessionId"]!.ToString()!);

            await mediator.Send(new SendTestAnswerCommand 
            { 
                SessionId = sessionId,
                QuestionAnswersId = request.QuestionAnswersId,
                QuestionId = request.QuestionId
            }, 
            cancellationToken);


            return NoContent();
        }
    }
}
