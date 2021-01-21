using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Business.Tests.Services.UserServiceTests.DeleteAsync
{
    [TestFixture]
    public class WhenDeletingUser : UserMetadataSearchStrategyTestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            await ClassInTest.DeleteAsync(ValidUser.Id, TestCancellationToken);
        }

        [Test]
        public void User_Is_Deleted()
        {
            FileStore.Verify(
                s => s.DeleteAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
        }
    }
}