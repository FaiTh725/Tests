namespace Authorization.Application.Contracts.User
{
    public class UserResponse
    {
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
    }
}
