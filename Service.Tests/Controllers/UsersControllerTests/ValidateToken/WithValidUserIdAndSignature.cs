using Glasswall.IdentityManagementService.Common.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.ValidateToken
{
    [TestFixture]
    public class WithValidUserIdAndSignature : UsersControllerTestBase
    {
        private string _input;
        private IActionResult _output;

        [OneTimeSetUp]
        public void Setup()
        {
            CommonSetup();

            _input = "some toke";

            TokenService.Setup(s => s.ValidateSignature(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            _output = ClassInTest.ValidateToken(new ValidateResetTokenModel
            {
                Token = _input
            });
        }

        [Test]
        public void Ok_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .WithPropEqual("message", "Token is valid"));
        }
    }
}