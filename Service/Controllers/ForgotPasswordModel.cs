using System.ComponentModel.DataAnnotations;

namespace Glasswall.IdentityManagementService.Api.Controllers
{
    public class ForgotPasswordModel
    {
        [Required] [MinLength(1)] public string Username { get; set; }
    }
}