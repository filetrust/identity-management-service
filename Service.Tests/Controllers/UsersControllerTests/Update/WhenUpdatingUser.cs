using System;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace Service.Tests.Controllers.UsersControllerTests.Update
{
    [TestFixture]
    public class WhenUpdatingUser : UsersControllerTestBase
    {
        private UpdateModel _input;
        private IActionResult _output;
        private string _validToken;

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
                    s.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ValidUser);

            TokenService.Setup(s => s.GetToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(_validToken = "Some token");

            _output = await ClassInTest.Update(ValidUser.Id, _input, TestCancellationToken);
        }


        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkResult>());
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
