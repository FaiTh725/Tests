using Authorization.Domain.Interfaces;
using MediatR;

namespace Authorization.Application.Commands.RefreshTokenEntity.DeleteRefreshToken
{
    public class DeleteRefreshTokenHandler
        : IRequestHandler<DeleteRefreshTokenCommand>
    {
        private readonly IUnitOfWork unitOfWork;

        public DeleteRefreshTokenHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            await unitOfWork.RefreshTokenRepository
                .RemoveToken(request.RefreshToken);
        }
    }
}
