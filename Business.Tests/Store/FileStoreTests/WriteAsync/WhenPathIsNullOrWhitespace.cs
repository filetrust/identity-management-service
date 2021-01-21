using System.Threading;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.WriteAsync
{
    [TestFixture]
    public class WhenPathIsNullOrWhitespace : FileStoreTestBase
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
        public void Exception_Is_Thrown(string testPath)
        {
            Assert.That(async () => await ClassInTest.WriteAsync(testPath, null, CancellationToken.None),
                ThrowsArgumentException("relativePath", "Value must not be null or whitespace"));
        }
    }
}