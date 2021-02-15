using System;

namespace Glasswall.IdentityManagementService.Common.Models.Store
{
    public class DefaultUser : User
    {
        public const string DefaultUserName = "default-administrator";
        public const string DefaultPassword = "administrator1";
        public static Guid DefaultUserId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        public DefaultUser()
        {
            Id = DefaultUserId;
            Status = UserStatus.Active;
            Username = DefaultUserName;
            Email = "";
            FirstName = "default";
            LastName = "user";
        }
    }
}