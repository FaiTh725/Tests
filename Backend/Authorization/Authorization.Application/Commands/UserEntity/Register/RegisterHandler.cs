using Application.Shared.Exceptions;
using Authorization.Application.Common.Interfaces;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Validators;
using MediatR;
using Microsoft.Extensions.Configuration;
using MassTransit;
using Authorization.Contracts.Events.User;


namespace Authorization.Application.Commands.UserEntity.Register
{
    public class RegisterHandler :
        IRequestHandler<RegisterCommand, (long, string)>
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHashService hashService;
        private readonly IJwtService<UserTokenRequest, UserTokenResponse> tokenService;
        private readonly IConfiguration configuration;
        private readonly IBus bus;
        
        public RegisterHandler(
            IUnitOfWork unitOfWork, 
            IHashService hashService,
            IJwtService<UserTokenRequest, UserTokenResponse> tokenService,
            IConfiguration configuration,
            IBus bus)
        {
            this.unitOfWork = unitOfWork;
            this.hashService = hashService;
            this.tokenService = tokenService;
            this.configuration = configuration;
            this.bus = bus;
        }

        public async Task<(long, string)> Handle(
            RegisterCommand request, 
            CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserRepository
                .GetUserByEmail(request.Email, cancellationToken);
        
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

            try
            {
                await unitOfWork.BeginTransactionAsync(cancellationToken);

                var userDb = await unitOfWork.UserRepository
                    .AddUser(userEntity.Value, cancellationToken);

                await bus.Publish<IUserCreated>(new
                {
                    CorrelationId = NewId.NextGuid(),
                    Email = request.Email,
                    Name = request.UserName
                }, 
                cancellationToken);

                var tokenLifeTime = configuration
                    .GetValue<int>("JwtSettings:ExpirationTimeRefreshTokenInDays");

                var refreshToken = tokenService.GenerateRefreshToken();
                var refreshTokenEntity = RefreshToken.Initialize(
                    refreshToken, userDb, DateTime.UtcNow.AddDays(tokenLifeTime));

                if (refreshTokenEntity.IsFailure)
                {
                    await unitOfWork.RollBackTransactionAsync(cancellationToken);
                    throw new InternalServerErrorException("Error initialize refresh token");
                }

                await unitOfWork.RefreshTokenRepository
                    .AddRefreshToken(refreshTokenEntity.Value, cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return (userDb.Id, refreshToken);
            }
            catch
            {
                await unitOfWork.RollBackTransactionAsync(cancellationToken);
                throw new InternalServerErrorException("Critical Server Error");
            }
        }
    }
}
