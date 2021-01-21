using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.ForgotPassword
{
    [TestFixture]
    public class WhenUsernameDoesNotMatch : UsersControllerTestBase
    {
        private ForgotPasswordModel _input;
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new ForgotPasswordModel
            {
                Username = "SOME OTHER USER"
            };

            UserService.Setup(s =>
                    s.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns(new[] {ValidUser}.AsAsyncEnumerable());

            _output = await ClassInTest.ForgotPassword(_input, TestCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<BadRequestObjectResult>()
                .With.Property(nameof(BadRequestObjectResult.Value))
                .WithPropEqual("message", $"{_input.Username} was not found"));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(
                x => x.GetAllAsync(It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
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