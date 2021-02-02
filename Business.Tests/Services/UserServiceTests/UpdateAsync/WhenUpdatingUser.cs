using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Glasswall.IdentityManagementService.Common.Store;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.UpdateAsync
{
    [TestFixture]
    public class WhenUpdatingUser : UserMetadataSearchStrategyTestBase
    {
        private UserEditOperationState _output;
        private User _inputUser;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] {$"{ValidUser.Id}.json"}.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ValidUser))));
            _inputUser = CreateUser();
            _inputUser.Id = ValidUser.Id;
            _output = await ClassInTest.UpdateAsync(_inputUser, TestCancellationToken);
        }

        [Test]
        public void Output_Is_Correct()
        {
            Assert.That(_output.User.Id, Is.EqualTo(_inputUser.Id));
            Assert.That(_output.User.Email, Is.EqualTo(_inputUser.Email));
            Assert.That(_output.User.FirstName, Is.EqualTo(_inputUser.FirstName));
            Assert.That(_output.User.LastName, Is.EqualTo(_inputUser.LastName));
            Assert.That(_output.User.Username, Is.EqualTo(_inputUser.Username));
            Assert.That(_output.User.Status, Is.EqualTo(_inputUser.Status));
        }

        [Test]
        public void Store_Is_Called_Correclty()
        {
            FileStore.Verify(
                s => s.ReadAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Exactly(2));

            FileStore.Verify(
                s => s.SearchAsync(
                    It.Is<string>(f => f == string.Empty),
                    It.IsAny<IPathActions>(),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);

            FileStore.Verify(
                s => s.WriteAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.IsAny<byte[]>(),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);

            FileStore.Verify(
                s => s.ExistsAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Exactly(2));

            FileStore.VerifyNoOtherCalls();
        }
    }
}