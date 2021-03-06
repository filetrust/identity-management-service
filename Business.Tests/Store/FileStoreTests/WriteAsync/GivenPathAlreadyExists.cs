﻿using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.WriteAsync
{
    [TestFixture]
    public class GivenPathAlreadyExists : FileStoreTestBase
    {
        private byte[] _expectedBytes;
        private string _fullPath;

        [OneTimeSetUp]
        public async Task Setup()
        {
            SharedSetup();

            var relativePath = $"{Guid.NewGuid()}/{Guid.NewGuid()}.txt";
            _fullPath = $"{RootPath}{Path.DirectorySeparatorChar}{relativePath}";

            Directory.CreateDirectory(Path.GetDirectoryName(_fullPath));
            await File.WriteAllBytesAsync(_fullPath, _expectedBytes = new byte[] {0x00, 0x11});

            await ClassInTest.WriteAsync(relativePath, _expectedBytes, CancellationToken);
        }

        [Test]
        public void File_Is_Written()
        {
            CollectionAssert.AreEqual(_expectedBytes, File.ReadAllBytes(_fullPath));
        }
    }
}