namespace Application.Shared.Exceptions
{
    public class UnauthorizeException : ApiException
    {
        public UnauthorizeException(
            string message):
            base(message)
        {
            
        }
    }
}
