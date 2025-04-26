using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetProfileTests(
            long profileId, CancellationToken cancellationToken)
        {
            var tests = await mediator.Send(new GetProfileTestsQuery
            {
                ProfileId = profileId,
            },
            cancellationToken);

            return Ok(tests);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetProfileCreatedGroups(
            long profileId, CancellationToken cancellationToken)
        {
            var groups = await mediator.Send(
                new GetProfileCreatedGroupQuery 
                { 
                    ProfileId = profileId
                },
                cancellationToken);

            return Ok(groups);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetProfileJoinedGroups(
            long profileId, CancellationToken cancellationToken)
        {
            var groups = await mediator.Send(
                new GetProfileJoinedGroupQuery
                {
                    ProfileId = profileId
                },
                cancellationToken);

            return Ok(groups);
        }
    }
}
