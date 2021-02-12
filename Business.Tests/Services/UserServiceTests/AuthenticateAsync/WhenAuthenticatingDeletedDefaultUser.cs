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
    public class WhenAuthenticatingDeletedDefaultUser : UserMetadataSearchStrategyTestBase
    {
        private string _filePath;
        private User _output;
        private MemoryStream _memoryStream;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            var user = new DefaultUser { PasswordSalt = null, PasswordHash = null, Status = UserStatus.Deactivated };
            var expectedUserData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] {_filePath = $"/mnt/{DefaultUser.DefaultUserId}"}.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_memoryStream = new MemoryStream(expectedUserData));

            _output = await ClassInTest.AuthenticateAsync(
                DefaultUser.DefaultUserName, DefaultUser.DefaultPassword, TestCancellationToken);
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
    }
}