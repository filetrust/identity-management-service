using System.Security.Cryptography;

namespace Glasswall.IdentityManagementService.Business.Store
{
    public static class EncryptionUtils
    {
        public static byte[] GenerateSalt(int length)
        {
            var bytes = new byte[length];
            using (var b = new RNGCryptoServiceProvider())
            {
                b.GetBytes(bytes);
            }

            return bytes;
        }
    }
}