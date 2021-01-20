using System;
using System.Threading;
using Glasswall.IdentityManagementService.Business.Services;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Store;
using Moq;
using TestCommon;

namespace Business.Tests.Store.UserMetadataSearchStrategyTests
{
    public abstract class UserMetadataSearchStrategyTestBase : UnitTestBase<UserMetadataSearchStrategy>
    {
        protected void CommonSetup()
        {
            ClassInTest = new UserMetadataSearchStrategy();
        }
    }
}
