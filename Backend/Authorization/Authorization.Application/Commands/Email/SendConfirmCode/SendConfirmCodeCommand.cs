using MediatR;

namespace Authorization.Application.Commands.Email.SendConfirmCode
{
    public class SendConfirmCodeCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
