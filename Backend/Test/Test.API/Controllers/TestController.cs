using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Test;
using Test.API.Contracts.TestAccess;
using Test.Application.Commands.Test.CreateTest;
using Test.Application.Commands.Test.DeleteTest;
using Test.Application.Commands.Test.UpdateTest;
using Test.Application.Commands.TestAccessEntity.GiveAccessTest;
using Test.Application.Commands.TestAccessEntity.LimitTestAccess;
using Test.Application.Common.Interfaces;
using Test.Application.Queries.Test.GetTestInfoById;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IProfileService profileService;

        public TestController(
            IMediator mediator,
            IProfileService profileService)
        {
            this.mediator = mediator;
            this.profileService = profileService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> CreateTest(
            CreateTestRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profileFromToken = await profileService
                .DecodeToken(token, cancellationToken);

            var testId = await mediator.Send(new CreateTestCommand
            {
                ProfileId = profileFromToken.Id,
                Name = request.Name,
                Description = request.Description,
                IsPublic = request.IsPublic,
                TestType = request.TestType,
                DurationInMinutes = request.DurationInMinutes
            }, cancellationToken);

            var test = await mediator.Send(new GetTestInfoByIdQuery
            {
                Id = testId
            },
            cancellationToken);

            return Ok(test);
        }

        [HttpGet("[action]")]
        [Authorize]
        public async Task<IActionResult> GetTestInfo(
            long testId, CancellationToken cancellationToken)
        {
            var test = await mediator.Send(new GetTestInfoByIdQuery
            {
                Id = testId
            }, 
            cancellationToken);

            return Ok(test);
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteTest(
            long testId, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            await mediator.Send(new DeleteTestCommand
            {
                TestId = testId,
                OwnerId = profile.Id,
                Role = profile.Role
            },
            cancellationToken);

            return Ok();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateTest(
            UpdateTestRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            var updatedTestId = await mediator.Send(new UpdateTestCommand
            {
                TestId = request.TestId,
                Description = request.Description,
                IsPublic = request.IsPublic,
                Name = request.Name,
                OwnerId = profile.Id,
                Role = profile.Role
            }, cancellationToken);

            return Ok(updatedTestId);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> ProviderTestAccess(
            ProvideTestAccessRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            var testAccessId = await mediator.Send(new GiveAccessTestCommand
                {
                    AccessTargetEntityId = request.TargetEntityId,
                    TargetEntity = request.TargetAccessEntityType,
                    TestId = request.TestId,
                    OwnerId = profile.Id,
                    Role = profile.Role
                },
                cancellationToken);

            return Ok(testAccessId);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> LimitTestAccess(
            LimitTestAccessRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            await mediator.Send(new LimitTestAccessCommand
            {
                OwnerId = profile.Id,
                Role = profile.Role,
                TargetEntityId = request.TargetEntityId,
                TestId = request.TestId,
                TargetEntity = request.TargetAccessEntityType
            },
            cancellationToken);

            return Ok();
        }
    }
}
