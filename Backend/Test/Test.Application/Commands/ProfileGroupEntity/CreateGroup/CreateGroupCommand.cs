using MediatR;

namespace Test.Application.Commands.ProfileGroupEntity.CreateGroup
{
    public class CreateGroupCommand :
        IRequest<long>
    {
        public string GroupName { get; set; } = string.Empty;

        public long OwnerId { get; set; }
    }
}
