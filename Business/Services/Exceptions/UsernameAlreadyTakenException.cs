using System;

namespace Glasswall.IdentityManagementService.Business.Services.Exceptions
{
    [Serializable]
    public class UsernameAlreadyTakenException : Exception
    {
        public UsernameAlreadyTakenException(Guid otherUserId, string username) : base(
            $"username '{username}' is already taken by user with id '{otherUserId}'")
        {
        }
    }
}