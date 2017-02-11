
namespace WcfService
{
    using System.Security.Cryptography;
    using System.Text;

    public class HashExtentions
    {
        public static string ShaHash(string value)
        {
            var crypt = new SHA256Managed();
            var hash = new System.Text.StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(value), 0, Encoding.UTF8.GetByteCount(value));

            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            return hash.ToString();
        }
    }
}