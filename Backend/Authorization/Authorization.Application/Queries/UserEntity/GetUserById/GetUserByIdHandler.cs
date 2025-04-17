using Application.Shared.Exceptions;
using Authorization.Application.Contracts.User;
using Authorization.Domain.Interfaces;
using MediatR;

namespace Authorization.Application.Queries.UserEntity.GetUserById
{
    public class GetUserByIdHandler :
        IRequestHandler<GetUserByIdQuery, UserResponse>
    {
        private readonly IUnitOfWork unitOfWork;

        public GetUserByIdHandler(
            IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await unitOfWork.UserRepository
                .GetUserById(request.Id, cancellationToken);
        
            if(user is null)
            {
                throw new NotFoundException("User doesnt exist");
            }

            return new UserResponse
            {
                Email = user.Email,
                Role = user.RoleId,
                UserName = user.UserName
            };
        }
    }
}
