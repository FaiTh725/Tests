using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.ProfileGroupEntity;
using Test.API.Filters;
using Test.Application.Commands.ProfileGroupEntity.AddGroupMember;
using Test.Application.Commands.ProfileGroupEntity.CreateGroup;
using Test.Application.Commands.ProfileGroupEntity.DeleteGroup;
using Test.Application.Commands.ProfileGroupEntity.DeleteMembersGroup;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileGroupEntity.GetGroupById;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IMediator mediator;

        public GroupController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> CreateGroup(
            CreateGroupRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            var groupId = await mediator.Send(new CreateGroupCommand
            {
                GroupName = request.Name,
                OwnerId = profile.Id
            },
            cancellationToken);

            var group = await mediator.Send(new GetGroupByIdQuery 
            { 
                Id = groupId
            }, 
            cancellationToken);

            return Ok(group);
        }

        [HttpPatch("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> AddGroupMember(
            AddGroupMemberRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            await mediator.Send(new AddGroupMemberCommand
            {
                GroupId = request.GroupId,
                ProfileId = request.MemberId,
                OwnerId = profile.Id,
                Role = profile.Role
            },
            cancellationToken);

            return NoContent();
        }

        [HttpPatch("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> DeleteMembersGroup(
            DeleteMembersGroupRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            await mediator.Send(new DeleteMembersGroupCommand
            {
                GroupId = request.GroupId,
                MembersId = request.MembersId,
                OwnerId = profile.Id,
                Role = profile.Role
            },
            cancellationToken);

            return NoContent();
        }

        [HttpDelete("[action]")]
        [Authorize]
        [ServiceFilter(typeof(VerifyProfileFilter))]
        public async Task<IActionResult> DeleteGroup(
            DeleteGroupRequest request, CancellationToken cancellationToken)
        {
            var profile = (VerifiedProfile)HttpContext.Items["profile"]!;

            await mediator.Send(new DeleteGroupCommand
            {
                GroupId = request.GroupId,
                OwnerId = profile.Id,
                Role = profile.Role
            },
            cancellationToken);

            return NoContent();
        }
    }
}
