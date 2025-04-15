namespace Application.Shared.Exceptions
{
    public class ForbiddenAccessException : ApiException
    {
        public ForbiddenAccessException(
            string message) :
            base(message)
        {
            
        }
    }
}
