﻿namespace Application.Shared.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(
            string message) :
            base(message)
        {
            
        }
    }
}
