using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Business.Tests.Services.UserServiceTests.UpdatePasswordAsync
{
    [TestFixture]
    public class UpdatingUserPassword : UserMetadataSearchStrategyTestBase
    {
        private string _password;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            await ClassInTest.UpdatePasswordAsync(ValidUser, _password = "Password", TestCancellationToken);
        }

        [Test]
        public void New_Password_Is_Saved()
        {
            FileStore.Verify(s => s.WriteAsync(
                    It.Is<string>(f => f == $"{ValidUser.Id}.json"),
                    It.Is<byte[]>(f => BytesEqual(f, Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(ValidUser)))),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
            FileStore.VerifyNoOtherCalls();
        }

        private static bool BytesEqual(IEnumerable<byte> bytes1, IReadOnlyList<byte> bytes2)
        {
            return !bytes1.Where((disByte, index) => bytes2[index] != disByte).Any();
        }
    }
}