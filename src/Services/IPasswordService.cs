namespace Rickie.Homework.ShowcaseApp.Services
{
    /// <summary>
    ///     Interface for password verification
    /// </summary>
    public interface IPasswordService
    {
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);

        string Base64Decode(string password);

        bool IsBase64String(string s);
    }
}