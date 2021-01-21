using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.ReadAsync
{
    [TestFixture]
    public class GivenValidFile : FileStoreTestBase
    {
        private MemoryStream _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);

            var relativePath = $"{Guid.NewGuid()}.txt";

            if (!File.Exists(relativePath))
                await File.WriteAllTextAsync($"{RootPath}{Path.DirectorySeparatorChar}{relativePath}", "some text",
                    CancellationToken);

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
            using (_output)
            {
                var str = Encoding.UTF8.GetString(_output.ToArray());

                Assert.That(str, Is.EqualTo("some text"));
            }
        }
    }
}