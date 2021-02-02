using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.ResetPassword
{
    [TestFixture]
    public class WhenIdentifierIsInvalid : UsersControllerTestBase
    {
        private ResetPasswordModel _input;
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new ResetPasswordModel
            {
                Token = "Some token",
                Password = "Some Password"
            };

            TokenService.Setup(s => s.GetIdentifier(It.IsAny<string>()))
                .Returns("some identifier");

            _output = await ClassInTest.ResetPassword(_input, TestCancellationToken);
        }

        [Test]
        public void BadRequestObjectResult_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<BadRequestObjectResult>()
                .With.Property(nameof(BadRequestObjectResult.Value))
                .WithPropEqual("message", "Token identifier was not valid"));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.VerifyNoOtherCalls();
        }

        [Test]
        public void Token_Service_Is_Leveraged_Correctly()
        {
            TokenService.Verify(
                x => x.GetIdentifier(
                    It.Is<string>(f => f == _input.Token)),
                Times.Once);
            TokenService.VerifyNoOtherCalls();
        }

        [Test]
        public void Email_Service_Is_Leveraged_Correctly()
        {
            EmailService.VerifyNoOtherCalls();
        }
    }
}