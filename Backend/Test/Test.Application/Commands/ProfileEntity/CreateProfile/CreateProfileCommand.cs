using MediatR;

namespace Test.Application.Commands.ProfileEntity.CreateProfile
{
    public class CreateProfileCommand : IRequest<long>
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
