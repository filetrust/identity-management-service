using NUnit.Framework;

namespace Business.Tests.Services.UserServiceTests.UpdatePasswordAsync
{
    [TestFixture]
    public class WithInvalidInput : UserMetadataSearchStrategyTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();
        }

        [Test]
        public void Method_Is_Protected_From_User()
        {
            Assert.That(() => ClassInTest.UpdatePasswordAsync(null, "Password", TestCancellationToken),
                ThrowsArgumentNullException("user"));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Method_Is_Protected_From_Password(string testCase)
        {
            Assert.That(() => ClassInTest.UpdatePasswordAsync(ValidUser, testCase, TestCancellationToken),
                ThrowsArgumentException("password", "Value cannot be null or whitespace."));
        }
    }
}