using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Store;
using Moq;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.ReadAsync
{
    [TestFixture]
    public class GivenValidFileAndEncryptionIsEnabled : FileStoreTestBase
    {
        private MemoryStream _output;
        private Mock<IEncryptionHandler> _encrypter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _encrypter = new Mock<IEncryptionHandler>();
            SharedSetup(encrypter: _encrypter.Object);

            if (!Directory.Exists(RootPath)) Directory.CreateDirectory(RootPath);

            var relativePath = $"{Guid.NewGuid()}.txt";

            if (!File.Exists(relativePath))
                await File.WriteAllTextAsync($"{RootPath}{Path.DirectorySeparatorChar}{relativePath}", 
                    FileStore.EncryptedMarker + string.Join(null, Enumerable.Repeat("a", 16)) + "some text",
                    CancellationToken);

            _encrypter.Setup(s => s.DecryptAsync(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(Encoding.UTF8.GetBytes("some text"));

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