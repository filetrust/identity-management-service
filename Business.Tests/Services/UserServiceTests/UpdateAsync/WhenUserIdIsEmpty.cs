using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.UpdateAsync
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
            Assert.That(() => ClassInTest.UpdateAsync(null, TestCancellationToken),
                ThrowsArgumentNullException("user"));
        }
    }
}