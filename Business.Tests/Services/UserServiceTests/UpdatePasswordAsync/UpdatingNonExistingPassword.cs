using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Services.Exceptions;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.UpdatePasswordAsync
{
    [TestFixture]
    public class UpdatingNonExistingPassword : UserMetadataSearchStrategyTestBase
    {
        private UserEditOperationState _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] { $"{ValidUser.Id}.json" }.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _output = await ClassInTest.UpdatePasswordAsync(CreateUser(), "Password", TestCancellationToken);
        }

        [Test]
        public void New_Password_Is_Not_Saved()
        {
            FileStore.Verify(s => s.WriteAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void Output_Is_Correct()
        {
            Assert.That(_output.User, Is.Null);
            Assert.That(_output.Errors.SelectMany(f => f.Value), Has.One.Items.InstanceOf<UserNotFoundError>());
        }
    }
}