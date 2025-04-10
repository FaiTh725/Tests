﻿using CSharpFunctionalExtensions;

namespace Authorization.Domain.Entities
{
    public class RefreshToken : Entity
    {
        public string Token { get; private set; }

        public DateTime ExpireOn { get; private set; }

        public User User { get; private set; }
        public long UserId { get; private set; }

        // For EF
        public RefreshToken() {}

        private RefreshToken(
            string token,
            User user,
            DateTime expireOn)
        {
            Token = token;
            User = user;
            ExpireOn = expireOn;
        }

        public static Result<RefreshToken> Initialize(
            string token,
            User user,
            DateTime expireOn)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                return Result.Failure<RefreshToken>("Token is emprt or null");
            }

            if(user is null)
            {
                return Result.Failure<RefreshToken>("User is null");
            }

            if(expireOn < DateTime.UtcNow)
            {
                return Result.Failure<RefreshToken>("Expire time points on th past");
            }

            return Result.Success(new RefreshToken(
                token,
                user,
                expireOn));
        }
    }
}
