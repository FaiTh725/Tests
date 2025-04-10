using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Validators;
using MediatR;

namespace Authorization.Application.Commands.UserEntity.Register
{
    public class RegisterHandler :
        IRequestHandler<RegisterCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHashService hashService;

        public RegisterHandler(
            IUnitOfWork unitOfWork, 
            IHashService hashService)
        {
            this.unitOfWork = unitOfWork;
            this.hashService = hashService;
        }

        public async Task<long> Handle(
            RegisterCommand request, 
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserRepository
                .GetUserByEmail(request.Email);
        
            if(user is not null)
            {
                throw new ConflictException("User already registered");
            }

            if(!UserValidator.IsValidPassword(request.Password))
            {
                throw new BadRequestException("Password must has one letter and one number, " +
                    $"in range size from {UserValidator.MIN_PASSWORD_LENGHT} to {UserValidator.MAX_PASSWORD_LENGHT}");
            }

            var passwordHash = hashService.GenerateHash(request.Password);

            var userEntity = User.Initialize(
                request.UserName,
                request.Email,
                passwordHash,
                "User");

            if(userEntity.IsFailure)
            {
                throw new BadRequestException("Invalid Request - " +
                    userEntity.Error);
            }

            var userDb = await unitOfWork.UserRepository
                .AddUser(userEntity.Value);

            await unitOfWork.SaveChangesAsync();

            return userDb.Id;
        }
    }
}
