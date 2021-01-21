using System;
using System.IO;
using System.Text;
using System.Threading;
using Glasswall.IdentityManagementService.Business.Services.Exceptions;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Business.Tests.Services.UserServiceTests.UpdateAsync
{
    [TestFixture]
    public class WhenUpdatingNonExistentUser : UserMetadataSearchStrategyTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] { $"{ValidUser.Id}.json" }.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(ValidUser))));
        }

        [Test]
        public void Exception_Is_raised()
        {
            Assert.That(async() => await ClassInTest.UpdateAsync(new User { Id = Guid.NewGuid(), Username = ValidUser.Username }, TestCancellationToken), Throws.InstanceOf<UserNotFoundException>());
        }
    }
}