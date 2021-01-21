using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Email;
using Glasswall.IdentityManagementService.Common.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Glasswall.IdentityManagementService.Business.Services
{
    [ExcludeFromCodeCoverage] // This will need some work around
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(
            ILogger<EmailService> logger,
            IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task SendAsync(EmailModel emailModel, CancellationToken cancellationToken)
        {
            if (emailModel == null) throw new ArgumentNullException(nameof(emailModel));

            _logger.LogInformation($"Sending message from '{emailModel.EmailFrom}' to '{string.Join(";", emailModel.EmailTo)}'");
            
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(emailModel.EmailFrom ?? _configuration["EmailFrom"]));
            mimeMessage.To.Add(MailboxAddress.Parse(emailModel.EmailTo.FirstOrDefault()));
            mimeMessage.Subject = emailModel.Subject;
            mimeMessage.Body = new TextPart(TextFormat.Html) { Text = emailModel.Body };

            return InternalSendAsync(mimeMessage, cancellationToken);
        }

        private async Task InternalSendAsync(MimeMessage mimeMessage, CancellationToken cancellationToken)
        {
            using var client = new SmtpClient();

            await client.ConnectAsync(_configuration["SmtpHost"], int.Parse(_configuration["SmtpPort"]), SecureSocketOptions.StartTls, cancellationToken);

            await client.AuthenticateAsync(_configuration["SmtpUser"], _configuration["SmtpPass"], cancellationToken);

            await client.SendAsync(FormatOptions.Default, mimeMessage, cancellationToken);

            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}