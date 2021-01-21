using System;
using System.IO;
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

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.AuthenticateAsync
{
    [TestFixture]
    public class AuthenticatingNonExistingUser : UserMetadataSearchStrategyTestBase
    {
        private string _filePath;
        private User _output;
        private MemoryStream _memoryStream;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            const string testPassword = "MyPassword1234";
            var (salt, hash) = SaltAndHashPassword(testPassword);

            ValidUser.PasswordSalt = salt;
            ValidUser.PasswordHash = hash;

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] {_filePath = $"/mnt/{Guid.NewGuid()}"}.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_memoryStream =
                    new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ValidUser))));

            _output = await ClassInTest.AuthenticateAsync(
                "i do not exist", testPassword, TestCancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _memoryStream?.Dispose();
        }

        [Test]
        public void User_Is_Not_Returned()
        {
            Assert.That(_output, Is.Null);
        }

        [Test]
        public void FileStore_Is_Called_Correctly()
        {
            FileStore.Verify(s => s.SearchAsync(It.Is<string>(f => f == ""), It.IsAny<UserMetadataSearchStrategy>(),
                It.Is<CancellationToken>(f => f == TestCancellationToken)));
            FileStore.Verify(s => s.ExistsAsync(_filePath, It.Is<CancellationToken>(f => f == TestCancellationToken)));
            FileStore.Verify(s => s.ReadAsync(_filePath, It.Is<CancellationToken>(f => f == TestCancellationToken)));

            FileStore.VerifyNoOtherCalls();
        }

        private static (byte[], byte[]) SaltAndHashPassword(string password)
        {
            using var hmac = new HMACSHA512();
            return (hmac.Key, hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}