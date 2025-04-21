using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    }
}
