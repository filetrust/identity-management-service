using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using TestCommon;

namespace Business.Tests.Services.UserServiceTests.CreateAsync
{
    [TestFixture]
    public class WhenCreatingExistingUser : UserMetadataSearchStrategyTestBase
    {
        private string _filePath;
        private User _output;
        private MemoryStream _memoryStream;
        private User _otherUser;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] {_filePath = $"{ValidUser.Id}.json"}.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_memoryStream =
                    new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ValidUser))));

            _output = await ClassInTest.CreateAsync(
                _otherUser = new User {Id = Guid.NewGuid(), Username = ValidUser.Username}, TestCancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _memoryStream?.Dispose();
        }

        [Test]
        public void User_Is_Returned()
        {
            Assert.That(_output.Id, Is.EqualTo(ValidUser.Id));
        }
    }
}