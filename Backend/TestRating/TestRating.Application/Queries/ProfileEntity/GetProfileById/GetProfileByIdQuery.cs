using MediatR;
using TestRating.Application.Contacts.Profile;

namespace TestRating.Application.Queries.ProfileEntity.GetProfileById
{
    public class GetProfileByIdQuery : IRequest<BaseProfileResponse>
    {
        public long Id { get; set; }
    }
}
