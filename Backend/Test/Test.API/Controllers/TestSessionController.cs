using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Test;
using Test.API.Filters;
using Test.Application.Commands.Test.SendTestAnswer;
using Test.Application.Commands.Test.StartTest;
using Test.Application.Commands.Test.StopTest;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.Test.GetTestToPass;

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
        public async Task<IActionResult> StopTest(
            StopTestCommand request, CancellationToken cancellationToken)
        {
            var testSession = await mediator.Send(request, cancellationToken);

            return Ok(testSession);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> SendTestAnswer(
            SendTestAnswerCommand request, CancellationToken cancellationToken)
        {
            await mediator.Send(request, cancellationToken);

            return NoContent();
        }
    }
}
