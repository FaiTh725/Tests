using CSharpFunctionalExtensions;

namespace Authorization.Domain.Entities
{
    public class Role
    {
        public string RoleName { get; private set; } = string.Empty;

        public List<User> Users { get; private set; } = new List<User>();

        // For EF
        public Role() { }

        private Role(string roleName)
        {
            RoleName = roleName;
        }

        public static Result<Role> Initialize(
            string roleName)
        {
            if(string.IsNullOrWhiteSpace(roleName))
            {
                return Result.Failure<Role>("Role is empty or null");
            }

            return Result.Success(new Role(roleName));
        }
    }
}
