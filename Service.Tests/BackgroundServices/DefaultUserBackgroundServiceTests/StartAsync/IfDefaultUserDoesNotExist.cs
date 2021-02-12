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
    public class IfDefaultUserDoesNotExist : DefaultUserBackgroundServiceTestBase
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
                    new User()
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
        public void The_Default_User_Is_Created()
        {
            UserService.Verify(f => f.CreateAsync(
                    It.IsAny<DefaultUser>(),
                    It.Is<CancellationToken>(x => x == CancellationToken)),
                Times.Once);
        }

        [Test]
        public void The_Default_Password_Is_Set()
        {
            UserService.Verify(f => f.UpdatePasswordAsync(
                    It.IsAny<DefaultUser>(),
                    It.Is<string>(x => x == DefaultUser.DefaultPassword),
                    It.Is<CancellationToken>(x => x == CancellationToken)),
                Times.Once);
        }
    }
}