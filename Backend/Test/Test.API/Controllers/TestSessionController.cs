using Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Test;
using Test.Application.Commands.Test.SendTestAnswer;
using Test.Application.Commands.Test.StartTest;
using Test.Application.Commands.Test.StopTest;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.GetProfileByEmail;
using Test.Application.Queries.Test.GetTestToPass;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestSessionController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ITokenService<ProfileToken> tokenService;

        public TestSessionController(
            IMediator mediator,
            ITokenService<ProfileToken> tokenService
            )
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> StartTest(
            StartTestRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var profile = await mediator.Send(new GetProfileByEmailQuery
            {
                Email = profileToken.Value.Email,
            }, cancellationToken);

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

            return Ok();
        }
    }
}
