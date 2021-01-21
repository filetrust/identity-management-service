using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Store;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using TestCommon;

namespace Business.Tests.Services.UserServiceTests.UpdateAsync
{
    [TestFixture]
    public class WhenUpdatingUser : UserMetadataSearchStrategyTestBase
    {
        private string _filePath;
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
                .ReturnsAsync(_memoryStream =
                    new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ValidUser))));

            await ClassInTest.UpdateAsync(ValidUser, TestCancellationToken);
        }

        [Test]
        public void Store_Is_Called_Correclty()
        {
            FileStore.Verify(
                s => s.ReadAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Exactly(2));

            FileStore.Verify(
                s => s.SearchAsync(
                    It.Is<string>(f => f == string.Empty),
                    It.IsAny<IPathActions>(),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);

            FileStore.Verify(
                s => s.WriteAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.IsAny<byte[]>(),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);

            FileStore.Verify(
                s => s.ExistsAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Exactly(2));

            FileStore.VerifyNoOtherCalls();
        }
    }
}