using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.DeleteAsync
{
    [TestFixture]
    public class GivenPathDoesNotExist : FileStoreTestBase
    {
        private string _fullPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            var relativePath = $"{Guid.NewGuid()}/{Guid.NewGuid()}.txt";

            _fullPath = $"{RootPath}{Path.DirectorySeparatorChar}{relativePath}";

            await ClassInTest.DeleteAsync(relativePath, CancellationToken);
        }

        [Test]
        public void File_Is_Deleted()
        {
            Assert.That(!File.Exists(_fullPath));
        }
    }
}