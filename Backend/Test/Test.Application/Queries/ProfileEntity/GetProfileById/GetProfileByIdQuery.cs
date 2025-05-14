using MediatR;
using Test.Application.Contracts.ProfileEntity;

namespace Test.Application.Queries.ProfileEntity.GetProfileById
{
    public class GetProfileByIdQuery : IRequest<ProfileResponse>
    {
        public long Id { get; set; }
    }
}
