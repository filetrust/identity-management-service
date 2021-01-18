using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Store;

namespace Glasswall.IdentityManagementService.Business.Store
{
    public class AesEncryptionHandler : IEncryptionHandler
    {
        public async Task<IEnumerable<byte>> EncryptAsync(byte[] data, byte[] key, byte[] iv, CancellationToken cancellationToken)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    return await PerformCryptography(data, encryptor, cancellationToken);
                }
            }
        }

        public async Task<IEnumerable<byte>> DecryptAsync(byte[] data, byte[] key, byte[] iv, CancellationToken cancellationToken)
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = key;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    return await PerformCryptography(data, decryptor, cancellationToken);
                }
            }
        }

        private static async Task<IEnumerable<byte>> PerformCryptography(byte[] data, ICryptoTransform cryptoTransform, CancellationToken cancellationToken)
        {
            await using (var ms = new MemoryStream())
            {
                await using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                {
                    await cryptoStream.WriteAsync(data, 0, data.Length, cancellationToken);
                    cryptoStream.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }
    }
}
