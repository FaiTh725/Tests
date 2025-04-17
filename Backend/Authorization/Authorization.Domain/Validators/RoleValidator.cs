namespace Authorization.Domain.Validators
{
    public static class RoleValidator
    {
        public static IEnumerable<string> Roles => ["Admin", "User"];
    }
}
