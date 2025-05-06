﻿namespace Application.Shared.Exceptions
{
    public class ConflictException : ApiException
    {
        public ConflictException(
            string message) :
            base(message)
        {
            
        }
    }
}
