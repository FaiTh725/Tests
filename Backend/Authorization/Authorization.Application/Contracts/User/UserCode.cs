namespace Authorization.Application.Contracts.User
{
    public class UserCode
    {
        public string Code { get; set; } = string.Empty;

        public DateTime SendingTime { get; set; }
    }
}
