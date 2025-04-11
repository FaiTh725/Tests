using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Validators;
using MediatR;

namespace Authorization.Application.Commands.UserEntity.Register
{
    public class RegisterHandler :
        IRequestHandler<RegisterCommand, (long, string)>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHashService hashService;
        private readonly IJwtService<UserTokenRequest, UserTokenResponse> tokenService;

        public RegisterHandler(
            IUnitOfWork unitOfWork, 
            IHashService hashService,
            IJwtService<UserTokenRequest, UserTokenResponse> tokenService)
        {
            this.unitOfWork = unitOfWork;
            this.hashService = hashService;
            this.tokenService = tokenService;
        }

        public async Task<(long, string)> Handle(
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

            await unitOfWork.BeginTransactionAsync();

            var userDb = await unitOfWork.UserRepository
                .AddUser(userEntity.Value);

            var refreshToken = tokenService.GenerateRefreshToken();
            var refreshTokenEntity = RefreshToken.Initialize(
                refreshToken, userDb, DateTime.UtcNow.AddDays(15));

            if (refreshTokenEntity.IsFailure)
            {
                await unitOfWork.RollBackTransactionAsync();
                throw new InternalServerErrorException("Error initialize refresh token");
            }

            await unitOfWork.RefreshTokenRepository
                .AddRefreshToken(refreshTokenEntity.Value);

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();

            return (userDb.Id, refreshToken);
        }
    }
}
