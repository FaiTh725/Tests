using MediatR;

namespace Authorization.Application.Commands.Email.VerifyCode
{
    public class VerifyCodeCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;
    }
}
