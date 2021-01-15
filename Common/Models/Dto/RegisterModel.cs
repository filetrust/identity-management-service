using System.ComponentModel.DataAnnotations;

namespace Glasswall.IdentityManagementService.Common.Models.Dto
{
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Email { get; set; }
    }
}