using Glasswall.IdentityManagementService.Business.Store;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.UserMetadataSearchStrategyTests
{
    public abstract class UserMetadataSearchStrategyTestBase : UnitTestBase<UserMetadataSearchStrategy>
    {
        protected void CommonSetup()
        {
            ClassInTest = new UserMetadataSearchStrategy();
        }
    }
}