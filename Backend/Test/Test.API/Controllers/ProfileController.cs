using MediatR;
using Microsoft.AspNetCore.Mvc;
using Test.Application.Commands.ProfileEntity.CreateProfile;

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

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateProfile(
            CreateProfileCommand request, CancellationToken cancellationToken)
        {
            var profileId = await mediator.Send(request, cancellationToken);

            return Ok(profileId);
        }
    }
}
