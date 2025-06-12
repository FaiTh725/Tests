namespace Authorization.Domain.Validators
{
    public static class RoleValidator
    {
        public static IReadOnlyList<string> Roles { get; } = new List<string> { "Admin", "User" }.AsReadOnly();
    }
}
