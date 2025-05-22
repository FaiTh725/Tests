namespace Authorization.IntegrationTests.Configurations
{
    public class PostgresConf
    {
        public string Image { get; set; } = string.Empty;

        public string DatabaseName { get; set; } = string.Empty;

        public string User {  get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int Port { get; set; }
    }
}
