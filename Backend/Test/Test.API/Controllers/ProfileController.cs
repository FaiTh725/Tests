using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.Common;
using Test.API.Filters;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.GetAvailableProfileTests;
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
        public async Task<IActionResult> GetProfileByFirstEmail(
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
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> GetProfileTests(
            [FromQuery]GetPaginatedDataRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            var query = new GetProfileTestsQuery
            {
                ProfileEmail = profile.Email,
                Page = request.Page,
                PageCount = request.PageSize
            };

            var tests = await mediator
                .Send(query, cancellationToken);

            return Ok(tests);
        }

        [HttpGet("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> GetProfilePrivateTests(
            [FromQuery] GetPaginatedDataRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            var query = new GetAvailableProfileTestsQuery
            { 
                ProfileId = profile.Id,
                Page = request.Page,
                PageSize = request.PageSize
            };

            var tests = await mediator.Send(query, cancellationToken);

            return Ok(tests);
        }

        [HttpGet("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> GetProfileCreatedGroups(
            [FromQuery]GetPaginatedDataRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            var query = new GetProfileCreatedGroupQuery
            {
                ProfileEmail = profile.Email,
                PageSize = request.PageSize,
                Page = request.Page
            };

            var groups = await mediator
                .Send(query, cancellationToken);

            return Ok(groups);
        }

        [HttpGet("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> GetProfileJoinedGroups(
            [FromQuery]GetPaginatedDataRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            var query = new GetProfileJoinedGroupQuery 
            { 
                Page = request.Page,
                PageSize = request.PageSize,
                ProfileEmail = profile.Email
            };

            var groups = await mediator
                .Send(query, cancellationToken);

            return Ok(groups);
        }
    }
}
