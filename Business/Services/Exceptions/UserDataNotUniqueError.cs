using System;
using Glasswall.IdentityManagementService.Common.Services;

namespace Glasswall.IdentityManagementService.Business.Services.Exceptions
{
    [Serializable]
    public class UserDataNotUniqueError : UserWriteError
    {
        public UserDataNotUniqueError(Guid otherUserId, string key, string keyValue) : base(
            key, nameof(UserDataNotUniqueError), $"{key}: '{keyValue}' is already taken by user with id '{otherUserId}'")
        {
        }
    }
}