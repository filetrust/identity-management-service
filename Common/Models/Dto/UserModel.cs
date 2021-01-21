using System;
using System.Diagnostics.CodeAnalysis;

namespace Glasswall.IdentityManagementService.Common.Models.Dto
{
    [ExcludeFromCodeCoverage]
    public class UserModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}