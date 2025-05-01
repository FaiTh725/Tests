using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TestRating.Application.Commands.ProfileEntity.AddProfile;

namespace TestRating.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMediator mediator;

        public TestController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            await mediator.Send(new AddProfileCommand 
            { 
                Email = "Test12@mail.ru",
                Name = "test123"
            });

            return Ok("Test successful");
        }
    }
}
