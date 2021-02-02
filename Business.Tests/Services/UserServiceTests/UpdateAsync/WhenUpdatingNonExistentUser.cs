using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Services.Exceptions;
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
    public class WhenUpdatingNonExistentUser : UserMetadataSearchStrategyTestBase
    {
        private UserEditOperationState _output;
        private User _inputUser;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] { $"{ValidUser.Id}.json" }.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.Is<string>(f => f == $"{ValidUser.Id}.json"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ValidUser))));

            _output = await ClassInTest.UpdateAsync(_inputUser = CreateUser(), TestCancellationToken);
        }

        [Test]
        public void Output_Is_Correct()
        {
            Assert.That(_output.User, Is.Null);
            Assert.That(_output.Errors.SelectMany(f => f.Value), Has.One.Items.InstanceOf<UserNotFoundError>());
        }

        [Test]
        public void Store_Is_Called_Correclty()
        {
            FileStore.Verify(
                s => s.ExistsAsync(
                    It.Is<string>(f => f == $"{_inputUser.Id}.json"),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Exactly(1));

            FileStore.VerifyNoOtherCalls();
        }
    }
}