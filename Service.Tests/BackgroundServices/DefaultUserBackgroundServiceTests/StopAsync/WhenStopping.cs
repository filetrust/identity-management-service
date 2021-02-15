using System.Threading.Tasks;
using NUnit.Framework;

namespace Service.Tests.BackgroundServices.DefaultUserBackgroundServiceTests.StopAsync
{
    [TestFixture]
    public class WhenStopping : DefaultUserBackgroundServiceTestBase
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            await ClassInTest.StopAsync(CancellationToken);
        }


        [Test]
        public void No_User_Operations_Occur()
        {
            UserService.VerifyNoOtherCalls();
        }
    }
}
