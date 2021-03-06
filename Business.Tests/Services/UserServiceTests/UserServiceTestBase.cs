﻿using System;
using System.Threading;
using Glasswall.IdentityManagementService.Business.Services;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Store;
using Moq;
using TestCommon;

namespace Glasswall.IdentityManagementService.Business.Tests.Services.UserServiceTests
{
    public abstract class UserMetadataSearchStrategyTestBase : UnitTestBase<UserService>
    {
        protected Mock<IFileStore> FileStore;
        protected CancellationToken TestCancellationToken;
        protected User ValidUser;

        protected void CommonSetup()
        {
            FileStore = new Mock<IFileStore>();
            TestCancellationToken = new CancellationToken(false);

            ValidUser = CreateUser();

            ClassInTest = new UserService(
                FileStore.Object);
        }

        protected User CreateUser()
        {
            return new User
            {
                Username = Guid.NewGuid().ToString(),
                Email = $"{Guid.NewGuid()}@email.email",
                FirstName = "Mr",
                LastName = "Man",
                Id = Guid.NewGuid(),
                PasswordHash = new byte[] {0x00, 0x11},
                PasswordSalt = new byte[] {0x01, 0x13},
                Status = UserStatus.Active
            };
        }
    }
}