using System;
using System.Threading;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.ValidateToken
{
    [TestFixture]
    public class WithValidUserButInvalidSignature : UsersControllerTestBase
    {
        private string _input;
        private IActionResult _output;

        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();

            _input = "some toke";

            TokenService.Setup(s => s.ValidateSignature(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);

            _output = ClassInTest.ValidateToken(new ValidateResetTokenModel
            {
                Token = _input
            });
        }

        [Test]
        public void Unauthorized_Returned()
        {
            Assert.That(_output, Is.InstanceOf<UnauthorizedObjectResult>()
                .With.Property(nameof(UnauthorizedObjectResult.Value))
                .WithPropEqual("message", "Token signature does not match."));
        }
    }
}