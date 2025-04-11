namespace Authorization.Application.Common.Interfaces
{
    public interface IHashService
    {
        string GenerateHash(string inputStr);

        bool VerifyHash(string inputStr, string hash);
    }
}
