using System;
using System.Threading;
using Glasswall.IdentityManagementService.Api.Controllers;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Moq;
using TestCommon;

namespace Service.Tests.Controllers.UsersControllerTests
{
    public abstract class UsersControllerTestBase : UnitTestBase<UsersController>
    {
        protected IIdentityManagementServiceConfiguration IdentityManagementConfig;
        protected Mock<IUserService> UserService;
        protected Mock<ITokenService> TokenService;
        protected Mock<IEmailService> EmailService;
        protected CancellationToken TestCancellationToken;
        protected User ValidUser;

        protected void CommonSetup()
        {
            IdentityManagementConfig = new IdentityManagementServiceConfiguration
            {
                TokenLifetime = TimeSpan.FromDays(1),
                EncryptionSecret = "MaSecret",
                ManagementUIEndpoint = "https://localhost.com",
                TokenSecret = "YoSecret",
                UserStoreRootPath = "/mnt/path"
            };

            UserService = new Mock<IUserService>();
            TokenService = new Mock<ITokenService>();
            EmailService = new Mock<IEmailService>();

            TestCancellationToken = new CancellationToken(false);

            ValidUser = new User
            {
                Username = "Username1",
                Email = "email@email.email",
                FirstName = "Mr",
                LastName = "Man",
                Id = Guid.NewGuid(),
                PasswordHash = new byte[] {0x00, 0x11},
                PasswordSalt = new byte[] {0x01, 0x13},
                Status = UserStatus.Active
            };

            ClassInTest = new UsersController(
                IdentityManagementConfig,
                UserService.Object,
                TokenService.Object,
                EmailService.Object);
        }
    }
}