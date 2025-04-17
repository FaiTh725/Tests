namespace Test.Infastructure.Configurations
{
    public class JwtTokenConf
    {
        public string SecretKey { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;
    }
}
