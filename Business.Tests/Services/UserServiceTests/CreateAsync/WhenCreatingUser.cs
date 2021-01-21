using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Business.Tests.Services.UserServiceTests.CreateAsync
{
    [TestFixture]
    public class WhenCreatingUser : UserMetadataSearchStrategyTestBase
    {
        private string _filePath;
        private User _output;
        private MemoryStream _memoryStream;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            FileStore.Setup(s => s.SearchAsync(It.IsAny<string>(), It.IsAny<UserMetadataSearchStrategy>(),
                    It.IsAny<CancellationToken>()))
                .Returns(new[] {_filePath = $"{ValidUser.Id}.json"}.AsAsyncEnumerable());

            FileStore.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            FileStore.Setup(s => s.ReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(ValidUser))));

            _output = await ClassInTest.CreateAsync(ValidUser, TestCancellationToken);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _memoryStream?.Dispose();
        }

        [Test]
        public void User_Is_Returned()
        {
            Assert.That(_output, Is.EqualTo(ValidUser));
        }

        [Test]
        public void FileStore_Is_Called_Correctly()
        {
            FileStore.Verify(s => s.SearchAsync(It.Is<string>(f => f == ""), It.IsAny<UserMetadataSearchStrategy>(),
                It.Is<CancellationToken>(f => f == TestCancellationToken)));

            FileStore.Verify(s => s.WriteAsync(
                    It.Is<string>(f => f == _filePath),
                    It.Is<byte[]>(f => BytesEqual(f, Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(ValidUser)))),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
            FileStore.Verify(s => s.ReadAsync(_filePath, It.Is<CancellationToken>(f => f == TestCancellationToken)));
            FileStore.Verify(s => s.ExistsAsync(_filePath, It.Is<CancellationToken>(f => f == TestCancellationToken)));

            FileStore.VerifyNoOtherCalls();
        }

        private static bool BytesEqual(IEnumerable<byte> bytes1, IReadOnlyList<byte> bytes2)
        {
            return !bytes1.Where((disByte, index) => bytes2[index] != disByte).Any();
        }
    }
}