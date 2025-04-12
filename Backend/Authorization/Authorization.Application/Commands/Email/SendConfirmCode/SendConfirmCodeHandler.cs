using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Interfaces;
using MassTransit;
using MediatR;
using Notification.Contracts.Email;
using System.Security.Cryptography;

namespace Authorization.Application.Commands.Email.SendConfirmCode
{
    public class SendConfirmCodeHandler :
        IRequestHandler<SendConfirmCodeCommand>
    {
        private readonly ICacheService cacheService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IBus bus;

        public SendConfirmCodeHandler(
            ICacheService cacheService,
            IUnitOfWork unitOfWork,
            IBus bus)
        {
            this.unitOfWork = unitOfWork;
            this.cacheService = cacheService;
            this.bus = bus;
        }

        public async Task Handle(
            SendConfirmCodeCommand request, 
            CancellationToken cancellationToken)
        {
            var existedUser = await unitOfWork.UserRepository
                .GetUserByEmail(request.Email, cancellationToken);

            if( existedUser is not null )
            {
                throw new ConflictException("Email already registered");
            }

            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[4];
            rng.GetBytes(randomBytes);
            int num = BitConverter.ToInt32(randomBytes, 0) & 0x7FFFFFFF;
            string code = (num % 1000000).ToString("D6");

            await bus.Publish(new SendEmailRequest
            {
                Consumer = request.Email,
                Subject = "Testing Confirm Email",
                Message = "Your confirmation code - " + code
            }, cancellationToken);

            await cacheService.SetData("unconfirmed_mail:" + request.Email, 
                new UserCode
                {
                    Code = code,
                    SendingTime = DateTime.UtcNow
                },
                360,
                cancellationToken);
        }
    }
}
