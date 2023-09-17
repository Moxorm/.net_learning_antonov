using System.Security.Cryptography;
using System.Text;

namespace webapi.Helpers
{
    public static class Password
    {
        public static string HashPassword(this string password)  
        {
            using (var hasher = SHA256.Create())
            {
                var hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = BitConverter.ToString(hashBytes).Replace("-","").ToLower();
                return hash;
            }
        }
    }
}
