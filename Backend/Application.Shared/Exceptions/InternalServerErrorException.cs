namespace Application.Shared.Exceptions
{
    public class InternalServerErrorException : ApiException
    {
        public InternalServerErrorException(
            string message) :
            base(message)
        {
            
        }
    }
}
