﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Dto;
using Glasswall.IdentityManagementService.Common.Models.Email;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests.New
{
    [TestFixture]
    public class AddingNewUser : UsersControllerTestBase
    {
        private RegisterModel _input;
        private IActionResult _output;
        private string _validToken;

        [OneTimeSetUp]
        public async Task Setup()
        {
            CommonSetup();

            _input = new RegisterModel
            {
                Username = ValidUser.Username,
                Email = ValidUser.Email,
                FirstName = ValidUser.FirstName,
                LastName = ValidUser.LastName
            };

            UserService.Setup(s =>
                    s.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserEditOperationState(ValidUser));

            TokenService.Setup(s => s.GetToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .Returns(_validToken = "Some token");

            _output = await ClassInTest.New(_input, TestCancellationToken);
        }


        [Test]
        public void Ok_Is_Returned()
        {
            Assert.That(_output, Is.InstanceOf<OkObjectResult>()
                .With.Property(nameof(OkObjectResult.Value))
                .WithPropEqual("message",
                    "Registration successful, please check your email for verification instructions"));
        }

        [Test]
        public void User_Service_Is_Leveraged_Correctly()
        {
            UserService.Verify(
                x => x.CreateAsync(
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
            TokenService.Verify(
                x => x.GetToken(
                    It.Is<string>(f => f == ValidUser.Id.ToString()),
                    It.Is<string>(f => f == string.Join(null, ValidUser.PasswordHash)),
                    It.Is<TimeSpan>(f => f == IdentityManagementConfig.TokenLifetime)),
                Times.Once);
            TokenService.VerifyNoOtherCalls();
        }

        [Test]
        public void Email_Service_Is_Leveraged_Correctly()
        {
            EmailService.Verify(x => x.SendAsync(It.Is<NewUserEmail>(
                        f => f.Subject == "New user notification"
                             && f.Body ==
                             $"Please confirm your email <a href=\"{IdentityManagementConfig.ManagementUIEndpoint}/confirm?token={_validToken}\">here</a>"
                             && f.EmailFrom == "admin@glasswallsolutions.com"
                             && f.EmailTo.Contains(ValidUser.Email)),
                    It.Is<CancellationToken>(f => f == TestCancellationToken)),
                Times.Once);

            EmailService.VerifyNoOtherCalls();
        }
    }
}