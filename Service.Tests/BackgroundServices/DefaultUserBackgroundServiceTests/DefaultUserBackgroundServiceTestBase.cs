using System.Threading;
using Glasswall.IdentityManagementService.Api.BackgroundServices;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.Extensions.Logging;
using Moq;
using TestCommon;

namespace Service.Tests.BackgroundServices.DefaultUserBackgroundServiceTests
{
    public abstract class DefaultUserBackgroundServiceTestBase : UnitTestBase<DefaultUserBackgroundService>
    {
        protected Mock<ILogger<DefaultUserBackgroundService>> Logger;
        protected Mock<IUserService> UserService;

        protected CancellationToken CancellationToken;

        protected void CommonSetup()
        {
            Logger = new Mock<ILogger<DefaultUserBackgroundService>>();
            UserService = new Mock<IUserService>();

            CancellationToken = new CancellationToken(false);

            ClassInTest = new DefaultUserBackgroundService(Logger.Object, UserService.Object);
        }
    }
}