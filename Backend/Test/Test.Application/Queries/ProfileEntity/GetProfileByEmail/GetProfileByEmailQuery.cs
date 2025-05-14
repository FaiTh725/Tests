using MediatR;
using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Queries.ProfileEntity.GetProfileByEmail
{
    public class GetProfileByEmailQuery : 
        IRequest<ProfileResponse>
    {
        public string Email { get; set; } = string.Empty;
    }
}
