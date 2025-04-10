using Authorization.Application.Contracts.User;
using MediatR;

namespace Authorization.Application.Queries.UserEntity.GetUserById
{
    public class GetUserByIdQuery : 
        IRequest<UserResponse>
    {
        public long Id { get; set; }
    }
}
