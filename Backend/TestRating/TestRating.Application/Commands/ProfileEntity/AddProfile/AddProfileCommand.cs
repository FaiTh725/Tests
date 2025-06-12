using MediatR;

namespace TestRating.Application.Commands.ProfileEntity.AddProfile
{
    public class AddProfileCommand : IRequest<long>
    {
        public string Email { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
