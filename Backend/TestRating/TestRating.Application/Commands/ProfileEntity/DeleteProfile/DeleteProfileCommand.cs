using MediatR;

namespace TestRating.Application.Commands.ProfileEntity.DeleteProfile
{
    public class DeleteProfileCommand : IRequest
    {
        public long Id { get; set; }
    }
}
