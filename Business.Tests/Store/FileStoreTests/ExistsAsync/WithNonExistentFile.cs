using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.ExistsAsync
{
    [TestFixture]
    public class WithNonExistentFile : FileStoreTestBase
    {
        private bool _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);

            var fullPath = $"{RootPath}/{Guid.NewGuid()}.txt";

            _output = await ClassInTest.ExistsAsync(fullPath, CancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void True_Is_Returned()
        {
            Assert.That(_output, Is.False);
        }
    }
}