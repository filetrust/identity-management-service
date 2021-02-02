using System;
using Glasswall.IdentityManagementService.Common.Services;

namespace Glasswall.IdentityManagementService.Business.Services.Exceptions
{
    [Serializable]
    public class UserNotFoundError : UserWriteError
    {
        public UserNotFoundError(Guid otherUserId)
            : base("Id", nameof(UserNotFoundError), $"User with id '{otherUserId}' cannot be found")
        {
        }
    }
}