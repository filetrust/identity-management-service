using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Service.Tests.Controllers.UsersControllerTests.Delete
{
    [TestFixture]
    public class WhenTokenIsNotSet : UsersControllerTestBase
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            CommonSetup(controllerContext);

            TokenService.Setup(s => s.GetIdentifier(It.IsAny<string>()))
                .Returns("SOME BAD ONE");
        }

        [Test]
        public void Exception_Is_Thrown()
        {
            Assert.That(() => ClassInTest.Delete(ValidUser.Id, TestCancellationToken),
                Throws.InvalidOperationException);
        }
    }
}