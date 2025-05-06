using Authorization.Application.Common.Interfaces;

namespace Authorization.Application.Common.Implementations
{
    public class HashService : IHashService
    {
        public string GenerateHash(string inputStr)
        {
            return BCrypt.Net.BCrypt.HashPassword(inputStr);
        }

        public bool VerifyHash(string inputStr, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(inputStr, hash);
        }
    }
}
