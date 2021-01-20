using System;
using NUnit.Framework;

namespace Business.Tests.Services.UserServiceTests.DeleteAsync
{
    [TestFixture]
    public class WithInvalidInputs : UserMetadataSearchStrategyTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();
        }

        [Test]
        public void An_Exception_Is_Thrown_For_User()
        {
            Assert.That(() => ClassInTest.DeleteAsync(Guid.Empty, TestCancellationToken), ThrowsArgumentException("id", "Value must not be empty"));
        }
    }
}