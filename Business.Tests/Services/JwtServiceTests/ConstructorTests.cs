using Business.Tests.Services.UserServiceTests;
using Glasswall.IdentityManagementService.Business.Services;
using NUnit.Framework;
using TestCommon;

namespace Business.Tests.Services.JwtServiceTests
{
    [TestFixture]
    public class ConstructorTests : JwtServiceTestBase
    {
        [Test]
        public void Constructs_With_Valid_Args()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<JwtTokenService>();
        }

        [Test]
        public void Throws_With_Null_Args()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<JwtTokenService>();
        }
    }
}