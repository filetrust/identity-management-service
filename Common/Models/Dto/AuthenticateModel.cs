using System.ComponentModel.DataAnnotations;

namespace Glasswall.IdentityManagementService.Common.Models.Dto
{
    public class AuthenticateModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}