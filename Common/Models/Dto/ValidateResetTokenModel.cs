using System.ComponentModel.DataAnnotations;

namespace Glasswall.IdentityManagementService.Common.Models.Dto
{
    public class ValidateResetTokenModel
    {
        [Required]
        public string Token { get; set; }
    }
}