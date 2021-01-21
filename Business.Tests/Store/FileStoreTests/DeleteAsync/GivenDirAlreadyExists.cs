using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.DeleteAsync
{
    [TestFixture]
    public class GivenDirAlreadyExists : FileStoreTestBase
    {
        private string _fullPath;
        private string _relativePath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            _relativePath = $"{Guid.NewGuid()}";
            _fullPath = $"{RootPath}{Path.DirectorySeparatorChar}{_relativePath}";

            Directory.CreateDirectory(_fullPath);

            await ClassInTest.DeleteAsync(_relativePath, CancellationToken);
        }

        [Test]
        public void File_Is_Deleted()
        {
            Assert.That(!Directory.Exists(_fullPath));
        }
    }
}