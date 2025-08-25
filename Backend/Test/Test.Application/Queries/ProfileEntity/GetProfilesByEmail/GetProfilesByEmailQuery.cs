using MediatR;
using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Queries.ProfileEntity.GetProfilesByEmail
{
    public class GetProfilesByEmailQuery : 
        IRequest<IEnumerable<ProfileResponse>>
    {
        public string Email { get; set; } = string.Empty;
    }
}
