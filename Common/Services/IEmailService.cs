using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Email;

namespace Glasswall.IdentityManagementService.Common.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailModel email, CancellationToken cancellationToken);
    }
}