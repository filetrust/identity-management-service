using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.ReadAsync
{
    [TestFixture]
    public class GivenEmptyFile : FileStoreTestBase
    {
        private MemoryStream _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);

            var relativePath = $"{Guid.NewGuid()}.txt";

            if (!File.Exists(relativePath))
            {
                await using (File.Create($"{RootPath}{Path.DirectorySeparatorChar}{relativePath}"))
                {
                }
            }

            _output = await ClassInTest.ReadAsync(relativePath, CancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Directory.Delete(RootPath, true);
        }

        [Test]
        public void Output_Is_File_Contents()
        {
            Assert.That(_output, Is.Null);
        }
    }
}