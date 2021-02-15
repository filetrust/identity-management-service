using Glasswall.IdentityManagementService.Api.BackgroundServices;
using NUnit.Framework;
using TestCommon;

namespace Service.Tests.BackgroundServices.DefaultUserBackgroundServiceTests
{
    [TestFixture]
    public class ConstructorTests : DefaultUserBackgroundServiceTestBase
    {
        [Test]
        public void Constructs_With_Valid_Args()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<DefaultUserBackgroundService>();
        }

        [Test]
        public void Throws_With_Null_Args()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<DefaultUserBackgroundService>();
        }
    }
}