﻿using MediatR;

namespace Authorization.Application.Commands.UserEntity.Login
{
    public class LoginCommand : IRequest<long>
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
