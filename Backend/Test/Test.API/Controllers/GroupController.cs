using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.ProfileGroupEntity;
using Test.Application.Commands.ProfileGroupEntity.AddGroupMember;
using Test.Application.Commands.ProfileGroupEntity.CreateGroup;
using Test.Application.Commands.ProfileGroupEntity.DeleteGroup;
using Test.Application.Commands.ProfileGroupEntity.DeleteMembersGroup;
using Test.Application.Common.Interfaces;
using Test.Application.Queries.ProfileGroupEntity.GetGroupById;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IProfileService profileService;

        public GroupController(
            IMediator mediator,
            IProfileService profileService)
        {
            this.mediator = mediator;
            this.profileService = profileService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> CreateGroup(
            CreateGroupRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var decodeProfile = await profileService
                .DecodeToken(token, cancellationToken);

            var groupId = await mediator.Send(new CreateGroupCommand
            {
                GroupName = request.Name,
                OwnerId = decodeProfile.Id
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
        public async Task<IActionResult> AddGroupMember(
            AddGroupMemberRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            await mediator.Send(new AddGroupMemberCommand
            {
                GroupId = request.GroupId,
                ProfileId = request.MemberId,
                OwnerId = profile.Id,
                Role = profile.Role
            },
            cancellationToken);

            return Ok();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteMembersGroup(
            DeleteMembersGroupRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            await mediator.Send(new DeleteMembersGroupCommand
            {
                GroupId = request.GroupId,
                MembersId = request.MembersId,
                OwnerId = profile.Id,
                Role = profile.Role
            },
            cancellationToken);

            return Ok();
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteGroup(
            DeleteGroupRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"];
            var profile = await profileService
                .DecodeToken(token, cancellationToken);

            await mediator.Send(new DeleteGroupCommand
            {
                GroupId = request.GroupId,
                OwnerId = profile.Id,
                Role = profile.Role
            },
            cancellationToken);

            return Ok();
        }
    }
}
