using Authorization.Domain.Interfaces;
using MediatR;

namespace Authorization.Application.Commands.RefreshTokenEntity.AddRefreshToken
{
    public class AddRefreshTokenHandler :
        IRequestHandler<AddRefreshTokenCommand, long>
    {
        private readonly IUnitOfWork unitOfWork;

        public AddRefreshTokenHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(AddRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return -1;
        }
    }
}
