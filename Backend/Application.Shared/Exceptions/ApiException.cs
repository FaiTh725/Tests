﻿namespace Application.Shared.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(
            string message) :
            base(message)
        {
            
        }
    }
}
