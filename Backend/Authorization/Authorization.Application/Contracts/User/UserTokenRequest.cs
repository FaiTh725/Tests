﻿namespace Authorization.Application.Contracts.User
{
    public class UserTokenRequest
    {
        public string Email { get; set; } = string.Empty; 
        
        public string Role { get; set; } = string.Empty;
    
        public string UserName { get; set; } = string.Empty;
    }
}
