using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CoinAmy
{
    internal class Hash
    {
        public static string HashPassword(string password)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] password_bytes = Encoding.UTF8.GetBytes(password);
            byte[] encrypted_bytes = sha256.ComputeHash(password_bytes);
            return Convert.ToBase64String(encrypted_bytes);
        }
    }
}
