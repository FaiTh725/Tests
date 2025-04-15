using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using MediatR;

namespace Authorization.Application.Commands.Email.VerifyCode
{
    public class VerifyCodeHandler :
        IRequestHandler<VerifyCodeCommand>
    {
        private readonly ICacheService cacheService;

        public VerifyCodeHandler(
            ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task Handle(
            VerifyCodeCommand request, 
            CancellationToken cancellationToken)
        {
            var userCode = await cacheService
                .GetData<UserCode>("unconfirmed_mail:" + request.Email, cancellationToken);

            if (userCode.IsFailure ||
                userCode.Value.Code != request.Code)
            {
                throw new BadRequestException("Invalid code or email");
            }

            await cacheService.SetData(
                "confirmed_email:" + request.Email, "Confirmed", 
                600, cancellationToken);

            await cacheService.RemoveData("unconfirmed_mail:" + request.Email, cancellationToken);
        }
    }
}
