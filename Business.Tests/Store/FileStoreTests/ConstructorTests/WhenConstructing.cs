using Glasswall.IdentityManagementService.Business.Store;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.FileStoreTests.ConstructorTests
{
    [TestFixture]
    public class WhenConstructing : UnitTestBase<FileStore>
    {
        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<FileStore>();
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Parameters()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<FileStore>();
        }
    }
}