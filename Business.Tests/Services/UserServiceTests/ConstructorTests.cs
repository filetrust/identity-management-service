using Glasswall.IdentityManagementService.Business.Services;
using NUnit.Framework;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests
{
    [TestFixture]
    public class ConstructorTests : UserMetadataSearchStrategyTestBase
    {
        [Test]
        public void Constructs_With_Valid_Args()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<UserService>();
        }

        [Test]
        public void Throws_With_Null_Args()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<UserService>();
        }
    }
}