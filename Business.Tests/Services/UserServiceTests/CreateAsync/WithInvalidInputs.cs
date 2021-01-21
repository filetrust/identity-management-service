using NUnit.Framework;

namespace Business.Tests.Services.UserServiceTests.CreateAsync
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
            Assert.That(() => ClassInTest.CreateAsync(null, TestCancellationToken),
                ThrowsArgumentNullException("user"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void An_Exception_Is_Thrown_For_Username(string testCase)
        {
            ValidUser.Username = testCase;
            Assert.That(() => ClassInTest.CreateAsync(ValidUser, TestCancellationToken),
                ThrowsArgumentException("Username", "Value is required"));
        }
    }
}