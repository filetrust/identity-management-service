using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using NUnit.Framework;
using TestCommon;

namespace Business.Tests.Store.AesEncryptionHandlerTests
{
    [TestFixture]
    public class AesEncryptionHandlerTests : UnitTestBase<AesEncryptionHandler>
    {
        [Test]
        public async Task With_Valid_Input_Can_Perform_Round_Trip()
        {
            ClassInTest = new AesEncryptionHandler();

            const string str = "SOME DATA TO ENCRYPT BECAUSE IT IS SO SENSITIVE GIVE IT A BREAK";
            var data = Encoding.UTF8.GetBytes(str);
            var key = Encoding.UTF8.GetBytes(string.Join(null, Enumerable.Repeat("!", 32).ToArray()));
            var salt = Encoding.UTF8.GetBytes(string.Join(null, Enumerable.Repeat("!", 16).ToArray()));

            var encrypted = await ClassInTest.EncryptAsync(data, key, salt, CancellationToken.None);
            var decrypted = await ClassInTest.DecryptAsync(encrypted.ToArray(), key, salt, CancellationToken.None);

            Assert.That(Encoding.UTF8.GetString(decrypted.ToArray()), Does.StartWith(str));
        }

        [Test]
        public void Salt_Generator_Works()
        {
            var salt = EncryptionUtils.GenerateSalt(32);

            Assert.That(salt, Has.Exactly(32).Items);
        }
    }
}
