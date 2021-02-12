using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests.DeleteAsync
{
    [TestFixture]
    public class WhenDeletingTheDefaultUser : UserMetadataSearchStrategyTestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            await ClassInTest.DeleteAsync(DefaultUser.DefaultUserId, TestCancellationToken);
        }

        [Test]
        public void Default_User_Is_Neutered_Instead_Of_Deleted()
        {
            var user = new DefaultUser { PasswordSalt = null, PasswordHash = null, Status = UserStatus.Deactivated };
            var expectedUserData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));
            
            FileStore.Verify(
                s => s.WriteAsync(
                    It.Is<string>(f => f == $"{DefaultUser.DefaultUserId}.json"),
                    It.Is<byte[]>(f => f.Select((b, i) => b == expectedUserData[i]).Any()),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
        }
    }
}