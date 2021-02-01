using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Api.Controllers;
using Glasswall.IdentityManagementService.Common.Models.Email;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.ForgotPassword
{
    [TestFixture]
    public class WhenUsernameMatches : UsersControllerTestBase
    {
        private ForgotPasswordModel _input;
        private IActionResult _output;
        private string _validToken;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new ForgotPasswordModel
            {
                Username = ValidUser.Username
            };

            UserService.Setup(s =>
                    s.GetAllAsync(It.IsAny<CancellationToken>()))
                .Returns(new[] {ValidUser}.AsAsyncEnumerable());

            TokenService.Setup(s => s.GetToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(_validToken = "Some token");

            _output = await ClassInTest.ForgotPassword(_input, TestCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .WithPropEqual("message",
                    "Password reset successful, please check your email for verification instructions"));
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
            TokenService.Verify(
                x => x.GetToken(
                    It.Is<string>(f => f == ValidUser.Id.ToString()),
                    It.Is<string>(f => f == string.Join(null, ValidUser.PasswordHash)),
                    It.Is<TimeSpan>(f => f == IdentityManagementConfig.TokenLifetime)),
                Times.Once);
            TokenService.VerifyNoOtherCalls();
        }

        [Test]
        public void Email_Service_Is_Leveraged_Correctly()
        {
            EmailService.Verify(x => x.SendAsync(It.Is<ForgotPasswordEmail>(
                        f => f.Subject == "Password reset notification"
                             && f.Body ==
                             $"Please reset your password <a href=\"{IdentityManagementConfig.ManagementUIEndpoint.TrimEnd('/')}/reset?Token={_validToken}\">here</a>"
                             && f.EmailFrom == "admin@glasswallsolutions.com"
                             && f.EmailTo.Contains(ValidUser.Email)),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);

            EmailService.VerifyNoOtherCalls();
        }
    }
}