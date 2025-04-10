using MediatR;

namespace Authorization.Application.Commands.UserEntity.Register
{
    public class RegisterCommand : IRequest<long>
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
    }
}
