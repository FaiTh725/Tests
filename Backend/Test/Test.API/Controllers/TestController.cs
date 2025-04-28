using Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Test;
using Test.API.Contracts.TestAccess;
using Test.Application.Commands.Test.CreateTest;
using Test.Application.Commands.Test.DeleteTest;
using Test.Application.Commands.Test.StartTest;
using Test.Application.Commands.Test.StopTest;
using Test.Application.Commands.Test.UpdateTest;
using Test.Application.Commands.TestAccessEntity.GiveAccessTest;
using Test.Application.Commands.TestAccessEntity.LimitTestAccess;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.GetProfileByEmail;
using Test.Application.Queries.Test.GetTestInfoById;
using Test.Application.Queries.Test.GetTestToPass;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ITokenService<ProfileToken> tokenService;

        public TestController(
            IMediator mediator,
            ITokenService<ProfileToken> tokenService)
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> CreateTest(
            CreateTestRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ?? 
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if(profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var profile = await mediator.Send(new GetProfileByEmailQuery
            {
                Email = profileToken.Value.Email
            }, cancellationToken);

            var testId = await mediator.Send(new CreateTestCommand
            {
                ProfileId = profile.Id,
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

        // add access
        [HttpGet("[action]")]
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
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            await mediator.Send(new DeleteTestCommand
            {
                TestId = testId,
                Email = profileToken.Value.Email,
                Role = profileToken.Value.Role
            },
            cancellationToken);

            return Ok();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public async Task<IActionResult> UpdateTest(
            UpdateTestRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if(profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var updatedTestId = await mediator.Send(new UpdateTestCommand
            {
                TestId = request.TestId,
                Description = request.Description,
                IsPublic = request.IsPublic,
                Name = request.Name,
                Email = profileToken.Value.Email,
                Role = profileToken.Value.Role
            }, cancellationToken);

            return Ok(updatedTestId);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> ProviderTestAccess(
            ProvideTestAccessRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
               throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var testAccessId = await mediator.Send(new GiveAccessTestCommand
                {
                    AccessTargetEntityId = request.TargetEntityId,
                    TargetEntity = request.TargetAccessEntityType,
                    TestId = request.TestId,
                    Email = profileToken.Value.Email,
                    Role = profileToken.Value.Role
                },
                cancellationToken);

            return Ok(testAccessId);
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> LimitTestAccess(
            LimitTestAccessRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
               throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            await mediator.Send(new LimitTestAccessCommand
            {
                Email = profileToken.Value.Email,
                Role = profileToken.Value.Role,
                TargetEntityId = request.TargetEntityId,
                TestId = request.TestId,
                TargetEntity = request.TargetAccessEntityType
            },
            cancellationToken);

            return Ok();
        }
    }
}
