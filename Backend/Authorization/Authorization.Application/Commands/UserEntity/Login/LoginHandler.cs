using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Domain.Interfaces;
using MediatR;

namespace Authorization.Application.Commands.UserEntity.Login
{
    public class LoginHandler :
        IRequestHandler<LoginCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHashService hashService;

        public LoginHandler(
            IUnitOfWork unitOfWork,
            IHashService hashService)
        {
            this.unitOfWork = unitOfWork;
            this.hashService = hashService;
        }

        public async Task<long> Handle(
            LoginCommand request, 
            CancellationToken cancellationToken)
        {
            var existedUser = await unitOfWork.UserRepository
                .GetUserByEmail(request.Email);

            if (existedUser is null)
            {
                throw new BadRequestException($"Email {request.Email} isnt registered");
            }

            if(!hashService.VerifyHash(
                request.Password,
                existedUser.PasswordHash))
            {
                throw new BadRequestException($"Email invalid password or email");
            }

            return existedUser.Id;
        }
    }
}
