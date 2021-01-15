using System;

namespace Glasswall.IdentityManagementService.Business.Services.Exceptions
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(Guid otherUserId) : base($"User with id '{otherUserId}' cannot be found")
        {
        }
    }
}