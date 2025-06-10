using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.Application.Queries.ProfileEntity.GetProfilesByEmail;
using Test.Application.Queries.ProfileGroupEntity.GetProfileCreatedGroup;
using Test.Application.Queries.ProfileGroupEntity.GetProfileJoinedGroup;
using Test.Application.Queries.Test.GetProfileTests;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProfileController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetProfileByFistEmail(
            string email, CancellationToken cancellation)
        {
            var profiles = await mediator.Send(
                new GetProfilesByEmailQuery
                {
                    Email = email
                },
                cancellation);

            return Ok(profiles);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetProfileTests(
            [FromQuery]GetProfileTestsQuery request, CancellationToken cancellationToken)
        {
            var tests = await mediator
                .Send(request, cancellationToken);

            return Ok(tests);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetProfileCreatedGroups(
            [FromQuery]GetProfileCreatedGroupQuery request, CancellationToken cancellationToken)
        {
            var groups = await mediator
                .Send(request, cancellationToken);

            return Ok(groups);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetProfileJoinedGroups(
            [FromQuery]GetProfileJoinedGroupQuery request, CancellationToken cancellationToken)
        {
            var groups = await mediator
                .Send(request, cancellationToken);

            return Ok(groups);
        }
    }
}
