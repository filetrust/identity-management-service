using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Store;
using Moq;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.WriteAsync
{
    [TestFixture]
    public class WhenEncryptionIsEnabled : FileStoreTestBase
    {
        private byte[] _expectedBytes;
        private string _fullPath;
        private Mock<IEncryptionHandler> _encrypter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _encrypter = new Mock<IEncryptionHandler>();
            SharedSetup(encrypter: _encrypter.Object);

            var relativePath = $"{Guid.NewGuid()}/{Guid.NewGuid()}.txt";
            _fullPath = $"{RootPath}{Path.DirectorySeparatorChar}{relativePath}";

            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
            await File.WriteAllBytesAsync(_fullPath, _expectedBytes = new byte[] {0x00, 0x11});

            _encrypter.Setup(s => s.EncryptAsync(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_expectedBytes);

            await ClassInTest.WriteAsync(relativePath, _expectedBytes, CancellationToken);
        }

        [Test]
        public void File_Is_Written()
        {
            var file = File.ReadAllBytes(_fullPath);

            var identifier = file.Where((_, i) => i < FileStore.EncryptedMarker.Length).ToArray();
            var salt = file.Skip(identifier.Length).Take(16);
            var encryptedContents = file.Skip(identifier.Length + 16);

            Assert.That(identifier, Has.Exactly(FileStore.EncryptedMarker.Length).Items);
            Assert.That(salt, Has.Exactly(16).Items);
            CollectionAssert.AreEqual(_expectedBytes, encryptedContents);
        }
    }
}