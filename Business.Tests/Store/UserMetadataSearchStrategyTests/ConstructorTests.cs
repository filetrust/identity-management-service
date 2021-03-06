﻿using Glasswall.IdentityManagementService.Business.Store;
using NUnit.Framework;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Store.UserMetadataSearchStrategyTests
{
    [TestFixture]
    public class ConstructorTests : UserMetadataSearchStrategyTestBase
    {
        [Test]
        public void Constructs_With_Valid_Args()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<UserMetadataSearchStrategy>();
        }

        [Test]
        public void Throws_With_Null_Args()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<UserMetadataSearchStrategy>();
        }
    }
}