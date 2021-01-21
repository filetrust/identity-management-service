using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.ExistsAsync
{
    [TestFixture]
    public class GivenInvalidInput : FileStoreTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SharedSetup();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Exception_Is_Thrown(string input)
        {
            Assert.That(async () => await ClassInTest.ExistsAsync(input, CancellationToken),
                ThrowsArgumentException("relativePath", "Value must not be null or whitespace"));
        }
    }
}