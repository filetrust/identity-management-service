using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.ValidateResetToken
{
    [TestFixture]
    public class WithValidUserIdAndSignature : UsersControllerTestBase
    {
        private ValidateResetTokenModel _input;
        private IActionResult _output;
        private string _validToken;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new ValidateResetTokenModel
            {
                Token = _validToken = "Some token"
            };

            UserService.Setup(s =>
                    s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ValidUser);

            TokenService.Setup(s => s.GetIdentifier(It.IsAny<string>()))
                .Returns(ValidUser.Id.ToString());

            TokenService.Setup(s => s.ValidateSignature(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _output = await ClassInTest.ValidateResetToken(_input, TestCancellationToken);
        }
        
        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .WithPropEqual("message", "Token is valid"));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(
                x => x.GetByIdAsync(
                    It.Is<Guid>(f => f == ValidUser.Id), 
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
            EmailService.VerifyNoOtherCalls();
        }
    }
}
