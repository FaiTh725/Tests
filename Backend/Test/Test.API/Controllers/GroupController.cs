using Application.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Test.API.Contracts.ProfileGroupEntity;
using Test.Application.Commands.ProfileGroupEntity.AddGroupMember;
using Test.Application.Commands.ProfileGroupEntity.CreateGroup;
using Test.Application.Commands.ProfileGroupEntity.DeleteGroup;
using Test.Application.Commands.ProfileGroupEntity.DeleteMembersGroup;
using Test.Application.Common.Interfaces;
using Test.Application.Contracts.ProfileEntity;
using Test.Application.Queries.ProfileEntity.GetProfileByEmail;
using Test.Application.Queries.ProfileGroupEntity.GetGroupById;

namespace Test.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly ITokenService<ProfileToken> tokenService;

        public GroupController(
            IMediator mediator,
            ITokenService<ProfileToken> tokenService)
        {
            this.mediator = mediator;
            this.tokenService = tokenService;
        }

        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> CreateGroup(
            CreateGroupRequest request, CancellationToken cancellationToken)
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
            }, 
            cancellationToken);

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
        public async Task<IActionResult> AddGroupMember(
            AddGroupMemberRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var profile = await mediator.Send(new GetProfileByEmailQuery
            {
                Email = profileToken.Value.Email
            }, 
            cancellationToken);

            await mediator.Send(new AddGroupMemberCommand
            {
                GroupId = request.GroupId,
                ProfileId = request.MemberId,
                Role = profileToken.Value.Role,
                OwnerId = profile.Id
            },
            cancellationToken);

            return Ok();
        }

        [HttpPatch("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteMembersGroup(
            DeleteMembersGroupRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var profile = await mediator.Send(new GetProfileByEmailQuery
            {
                Email = profileToken.Value.Email
            },
            cancellationToken);

            await mediator.Send(new DeleteMembersGroupCommand
            {
                GroupId = request.GroupId,
                MembersId = request.MembersId,
                OwnerId = profile.Id,
                Role = profileToken.Value.Role
            },
            cancellationToken);

            return Ok();
        }

        [HttpDelete("[action]")]
        [Authorize]
        public async Task<IActionResult> DeleteGroup(
            DeleteGroupRequest request, CancellationToken cancellationToken)
        {
            var token = Request.Cookies["token"] ??
                throw new UnauthorizedAccessException("User isnt authorized");

            var profileToken = tokenService.DecodeToken(token);

            if (profileToken.IsFailure)
            {
                throw new InternalServerErrorException("Invalid token signature");
            }

            var profile = await mediator.Send(new GetProfileByEmailQuery
            {
                Email = profileToken.Value.Email
            },
            cancellationToken);

            await mediator.Send(new DeleteGroupCommand
            {
                GroupId = request.GroupId,
                OwnerId = profile.Id,
                Role = profileToken.Value.Role
            },
            cancellationToken);

            return Ok();
        }
    }
}
