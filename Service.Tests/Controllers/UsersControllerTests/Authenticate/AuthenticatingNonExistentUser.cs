using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Service.Tests.Controllers.UsersControllerTests.Authenticate
{
    [TestFixture]
    public class AuthenticatingNonExistentUser : UsersControllerTestBase
    {
        private AuthenticateModel _input;
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new AuthenticateModel
            {
                Password = "Hunter2",
                Username = "Username1"
            };

            _output = await ClassInTest.Authenticate(_input, TestCancellationToken);
        }

        [Test]
        public void Unauthoirzed_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<UnauthorizedObjectResult>()
                .With.Property(nameof(UnauthorizedObjectResult.Value))
                .With.Property("message")
                .EqualTo("Username or password is incorrect"));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(
                x => x.AuthenticateAsync(
                    It.Is<string>(f => f == _input.Username),
                    It.Is<string>(f => f == _input.Password),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
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