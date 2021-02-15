using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.BackgroundServices.DefaultUserBackgroundServiceTests.StartAsync
{
    [TestFixture]
    public class IfDefaultUserDoesExist : DefaultUserBackgroundServiceTestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            UserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns(new[]
                {
                    new User(),
                    new User(),
                    new User(),
                    new User(),
                    new DefaultUser(),
                }.AsAsyncEnumerable());

            UserService.Setup(s => s.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserEditOperationState());

            await ClassInTest.StartAsync(CancellationToken);
        }

        [Test]
        public void The_Store_Is_Searched()
        {
            UserService.Verify(f => f.GetAllAsync(It.Is<CancellationToken>(x => x == CancellationToken)), Times.Once);
        }

        [Test]
        public void The_Default_User_Is_Not_Created()
        {
            UserService.Verify(f => f.CreateAsync(
                    It.IsAny<DefaultUser>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void The_Default_Password_Is_Not_Set()
        {
            UserService.Verify(f => f.UpdatePasswordAsync(
                    It.IsAny<DefaultUser>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}