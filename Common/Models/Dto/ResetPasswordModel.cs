using System.ComponentModel.DataAnnotations;

namespace Glasswall.IdentityManagementService.Common.Models.Dto
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}