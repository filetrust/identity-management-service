using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Service.Tests.Controllers.UsersControllerTests.Update
{
    [TestFixture]
    public class WhenUpdatingUserFails : UsersControllerTestBase
    {
        private UpdateModel _input;
        private IActionResult _output;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new UpdateModel
            {
                Username = ValidUser.Username,
                Email = ValidUser.Email,
                FirstName = ValidUser.FirstName,
                LastName = ValidUser.LastName
            };

            UserService.Setup(s =>
                    s.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserEditOperationState(ValidUser, new UserWriteError("key", "some error type", "some error")));

            TokenService.Setup(s => s.GetToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns("Some token");

            _output = await ClassInTest.Update(ValidUser.Id, _input, TestCancellationToken);
        }


        [Test]
        public void BadRequest_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(
                x => x.UpdateAsync(
                    It.Is<User>(f =>
                        f.Id != Guid.Empty && f.Username == ValidUser.Username &&
                        f.FirstName == ValidUser.FirstName && f.LastName == ValidUser.LastName &&
                        f.Email == ValidUser.Email),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);
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