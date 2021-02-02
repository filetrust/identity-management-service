using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.UpdatePasswordAsync
{
    [TestFixture]
    public class UpdatingUserPassword : UserMetadataSearchStrategyTestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] { $"{ValidUser.Id}.json" }.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.Is<string>(x => x == $"{ValidUser.Id}.json"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ValidUser))));
            
            await ClassInTest.UpdatePasswordAsync(ValidUser, "Password", TestCancellationToken);
        }

        [Test]
        public void New_Password_Is_Saved()
        {
            FileStore.Verify(s => s.WriteAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.Is<byte[]>(f => UploadingUserPasswordMatches(f)),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
        }

        private static bool UploadingUserPasswordMatches(byte[] bytes)
        {
            var deserialized = JsonConvert.DeserializeObject<User>(Encoding.UTF8.GetString(bytes));

            return PasswordMatches("Password", deserialized.PasswordHash, deserialized.PasswordSalt);
        }

        private static bool PasswordMatches(string password, IReadOnlyList<byte> storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return !computedHash.Where((t, i) => t != storedHash[i]).Any();
            }
        }
    }
}