using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.DeleteAsync
{
    [TestFixture]
    public class GivenDirDoesNotExist : FileStoreTestBase
    {
        private string _fullPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            var relativePath = $"{Guid.NewGuid()}/{Guid.NewGuid()}.txt";

            _fullPath = $"{RootPath}{Path.DirectorySeparatorChar}{relativePath}";

            await ClassInTest.DeleteAsync(Path.GetDirectoryName(_fullPath), CancellationToken);
        }

        [Test]
        public void Dir_Is_Deleted()
        {
            Assert.That(!Directory.Exists(Path.GetDirectoryName(_fullPath)));
        }
    }
}