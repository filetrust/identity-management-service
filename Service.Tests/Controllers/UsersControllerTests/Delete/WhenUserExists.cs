using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Service.Tests.Controllers.UsersControllerTests.Delete
{
    [TestFixture]
    public class WhenUserExists : UsersControllerTestBase
    {
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _output = await ClassInTest.Delete(ValidUser.Id, TestCancellationToken);
        }

        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkResult>());
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
            TokenService.VerifyNoOtherCalls();
        }

        [Test]
        public void Email_Service_Is_Leveraged_Correctly()
        {
            EmailService.VerifyNoOtherCalls();
        }
    }
}