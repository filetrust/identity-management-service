using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.SearchAsync
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
        public void Exception_Is_Thrown()
        {
            Assert.That(() => ClassInTest.SearchAsync("", null, CancellationToken),
                ThrowsArgumentNullException("pathActions"));
        }
    }
}