using System.Diagnostics.CodeAnalysis;

namespace Glasswall.IdentityManagementService.Common.Models.Dto
{
    [ExcludeFromCodeCoverage]
    public class UpdateModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}