using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.Authenticate
{
    [TestFixture]
    public class AuthenticatingExistingUser : UsersControllerTestBase
    {
        private AuthenticateModel _input;
        private IActionResult _output;
        private string _validToken;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new AuthenticateModel
            {
                Username = "Username1",
                Password = "Hunter2"
            };

            UserService.Setup(s =>
                    s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ValidUser);

            TokenService.Setup(s => s.GetToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(_validToken = "Some token");

            _output = await ClassInTest.Authenticate(_input, TestCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .WithPropEqual("Id", ValidUser.Id)
                .And.WithPropEqual("Username", ValidUser.Username)
                .And.WithPropEqual("FirstName", ValidUser.FirstName)
                .And.WithPropEqual("LastName", ValidUser.LastName)
                .And.WithPropEqual("token", _validToken));
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
            TokenService.Verify(
                x => x.GetToken(
                    It.Is<string>(f => f == ValidUser.Id.ToString()),
                    It.Is<string>(f => f == IdentityManagementConfig.TokenSecret),
                    It.Is<TimeSpan>(f => f == IdentityManagementConfig.TokenLifetime)),
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