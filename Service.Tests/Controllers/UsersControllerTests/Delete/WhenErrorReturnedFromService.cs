using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Service.Tests.Controllers.UsersControllerTests.Delete
{
    [TestFixture]
    public class WhenErrorReturnedFromService : UsersControllerTestBase
    {
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var httpContext = new DefaultHttpContext(); // or mock a `HttpContext`
            httpContext.Request.Headers["Authorization"] = "Bearer fake_token_here"; //Set header
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            CommonSetup(controllerContext);

            TokenService.Setup(s => s.GetIdentifier(It.IsAny<string>()))
                .Returns(Guid.NewGuid().ToString());

            UserService.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserEditOperationState(null, new UserWriteError("key", "error", "error")));

            _output = await ClassInTest.Delete(ValidUser.Id, TestCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(x => x.DeleteAsync(It.Is<Guid>(f => f == ValidUser.Id),
                It.Is<CancellationToken>(f => f == TestCancellationToken)));
            UserService.VerifyNoOtherCalls();
        }

        [Test]
        public void Token_Service_Is_Leveraged_Correctly()
        {
            TokenService.Verify(s => s.GetIdentifier(It.Is<string>(x => x == "fake_token_here")), Times.Once);
            TokenService.VerifyNoOtherCalls();
        }

        [Test]
        public void Email_Service_Is_Leveraged_Correctly()
        {
            EmailService.VerifyNoOtherCalls();
        }
    }
}