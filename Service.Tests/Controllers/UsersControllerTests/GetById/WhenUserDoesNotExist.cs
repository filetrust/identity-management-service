using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.GetById
{
    [TestFixture]
    public class WhenUserDoesNotExist : UsersControllerTestBase
    {
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();
            
            _output = await ClassInTest.GetById(ValidUser.Id, TestCancellationToken);
        }

        [Test]
        public void BadRequest_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<BadRequestObjectResult>()
                .With.Property(nameof(BadRequestObjectResult.Value))
                .WithPropEqual("message", "User does not exist"));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(x => x.GetByIdAsync(It.Is<Guid>(f => f == ValidUser.Id), It.Is<CancellationToken>(f => f == TestCancellationToken)));
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
