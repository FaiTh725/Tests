﻿namespace Application.Shared.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(
            string message) : 
            base(message)
        {
            
        }
    }
}
