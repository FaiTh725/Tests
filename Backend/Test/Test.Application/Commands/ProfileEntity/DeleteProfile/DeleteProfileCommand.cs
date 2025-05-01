using MediatR;

namespace Test.Application.Commands.ProfileEntity.DeleteProfile
{
    public class DeleteProfileCommand : IRequest
    {
        public long ProfileId { get; set; }
    }
}
