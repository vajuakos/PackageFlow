using System.Security.Cryptography;
using System.Text;

namespace PackageFlow.Core.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("Password parameter is null!");

            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        public static bool Verify(string password, string storedHash)
        {
            string hashOfInput = Hash(password);
            return string.Equals(hashOfInput, storedHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
