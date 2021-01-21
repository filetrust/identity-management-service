using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.GetAll
{
    [TestFixture]
    public class GettingAllUsers : UsersControllerTestBase
    {
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            UserService.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns(new[] {ValidUser}.AsAsyncEnumerable());

            _output = await ClassInTest.GetAll(TestCancellationToken);
        }


        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .With.One.Items
                .WithPropEqual("Id", ValidUser.Id)
                .And.WithPropEqual("FirstName", ValidUser.FirstName)
                .And.WithPropEqual("LastName", ValidUser.LastName)
                .And.WithPropEqual("Username", ValidUser.Username)
                .And.WithPropEqual("Email", ValidUser.Email)
                .And.WithPropEqual("Status", ValidUser.Status));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(x => x.GetAllAsync(It.Is<CancellationToken>(f => f == TestCancellationToken)));
            UserService.VerifyNoOtherCalls();
        }

        [Test]
        public void Token_Service_Is_Leveraged_Correctly()
        {
            TokenService.VerifyNoOtherCalls();
        }

        [Test]
        public void Email_Service_Is_Leveraged_Correctly()
        {
            EmailService.VerifyNoOtherCalls();
        }
    }
}
