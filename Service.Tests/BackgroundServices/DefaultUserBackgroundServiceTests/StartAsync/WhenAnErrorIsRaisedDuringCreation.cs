using System;
using System.Collections.Concurrent;
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
    public class WhenAnErrorIsRaisedDuringCreation : DefaultUserBackgroundServiceTestBase
    {
        private Exception _exception;

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
                .ReturnsAsync(new UserEditOperationState
                {
                    Errors = new ConcurrentDictionary<string, UserWriteError[]>
                    {
                        ["Some Error Type"] = new []
                        {
                            new UserWriteError("some error", "some error type", "some error message")
                        }
                    }
                });

            try
            {
                await ClassInTest.StartAsync(CancellationToken);
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
        }

        [Test]
        public void The_Store_Is_Searched()
        {
            UserService.Verify(f => f.GetAllAsync(It.Is<CancellationToken>(x => x == CancellationToken)), Times.Once);
        }

        [Test]
        public void The_Default_User_Creation_Is_Attempted()
        {
            UserService.Verify(f => f.CreateAsync(
                    It.IsAny<DefaultUser>(),
                    It.Is<CancellationToken>(x => x == CancellationToken)),
                Times.Once);
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

        [Test]
        public void Exception_Is_Rethrown()
        {
            Assert.That(_exception, Is.Not.Null);
        }
    }
}