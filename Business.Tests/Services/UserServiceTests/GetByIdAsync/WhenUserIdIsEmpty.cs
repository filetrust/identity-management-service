using System;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.GetByIdAsync
{
    [TestFixture]
    public class WhenUserIdIsEmpty : UserMetadataSearchStrategyTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();
        }

        [Test]
        public void Exception_Is_Thrown()
        {
            Assert.That(() => ClassInTest.GetByIdAsync(Guid.Empty, TestCancellationToken),
                ThrowsArgumentException("id", "Value must not be empty"));
        }
    }
}