namespace Application.Shared.Exceptions
{
    public class AppConfigurationException : ApiException
    {
        public string SectionWithError { get; private set; } = string.Empty;

        public AppConfigurationException(
            string sectionError,
            string message = "") :
            base(message)
        {
            SectionWithError = sectionError;
        }
    }
}
