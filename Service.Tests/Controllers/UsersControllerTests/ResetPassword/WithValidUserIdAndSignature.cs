using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Glasswall.IdentityManagementService.Common.Models.Email;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.ResetPassword
{
    [TestFixture]
    public class WithValidUserIdAndSignature : UsersControllerTestBase
    {
        private ResetPasswordModel _input;
        private IActionResult _output;
        private string _validToken;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new ResetPasswordModel
            {
                Token = _validToken = "Some token",
                Password = "Some new Password"
            };

            UserService.Setup(s =>
                    s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ValidUser);

            TokenService.Setup(s => s.GetIdentifier(It.IsAny<string>()))
                .Returns(ValidUser.Id.ToString());

            TokenService.Setup(s => s.ValidateSignature(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _output = await ClassInTest.ResetPassword(_input, TestCancellationToken);
        }
        
        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .WithPropEqual("message", "Password reset successful, you can now login"));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(
                x => x.GetByIdAsync(
                    It.Is<Guid>(f => f == ValidUser.Id), 
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
            UserService.Verify(
                x => x.UpdatePasswordAsync(
                    It.Is<User>(f => f == ValidUser),
                    It.Is<string>(f => f == _input.Password),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
            UserService.VerifyNoOtherCalls();
        }

        [Test]
        public void Token_Service_Is_Leveraged_Correctly()
        {
            TokenService.Verify(
                x => x.GetIdentifier(
                    It.Is<string>(f => f == _input.Token)),
                Times.Once);
            TokenService.Verify(
                x => x.ValidateSignature(
                    It.Is<string>(f => f == _input.Token),
                    It.Is<string>(f => f == string.Join(null, ValidUser.PasswordHash))),
                Times.Once);
            TokenService.VerifyNoOtherCalls();
        }

        [Test]
        public void Email_Service_Is_Leveraged_Correctly()
        {
            EmailService.Verify(x => x.SendAsync(It.Is<PasswordSetConfirmationEmail>(
                        f => f.Subject == "Your Password has been changed."
                             && f.Body ==
                             $"You have successfully set your password. Please log into your account <a href=\"{IdentityManagementConfig.ManagementUIEndpoint}\">here</a>."
                             && f.EmailFrom == "admin@glasswallsolutions.com"
                             && f.EmailTo.Contains(ValidUser.Email)),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
            EmailService.VerifyNoOtherCalls();
        }
    }
}
