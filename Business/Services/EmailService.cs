using System;
using System.Configuration;
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

            CheckConfigItem("SmtpHost");
            CheckConfigItem("SmtpPort");
            CheckConfigItem("SmtpUser");
            CheckConfigItem("SmtpPass");
            CheckConfigItem("SmtpSecureSocketOptions");
            
            _logger.LogInformation($"Sending message from '{emailModel.EmailFrom}' to '{string.Join(";", emailModel.EmailTo)}'");

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(_configuration["SmtpUser"]));
            mimeMessage.To.Add(MailboxAddress.Parse(emailModel.EmailTo.FirstOrDefault()));
            mimeMessage.Subject = emailModel.Subject;
            mimeMessage.Body = new TextPart(TextFormat.Html) {Text = emailModel.Body};

            return TryInternalSendAsync(mimeMessage, 0, cancellationToken);
        }

        private void CheckConfigItem(string key)
        {
            if (string.IsNullOrWhiteSpace(_configuration[key])) 
                throw new ConfigurationErrorsException($"{key} is not set");
        }

        private async Task TryInternalSendAsync(MimeMessage mimeMessage, int cTry, CancellationToken cancellationToken)
        {
            try
            {
                var host = _configuration["SmtpHost"];
                var port = int.Parse(_configuration["SmtpPort"]);
                var options = Enum.Parse<SecureSocketOptions>(_configuration["SmtpSecureSocketOptions"]);
                var user = _configuration["SmtpUser"];
                var pwd = _configuration["SmtpPass"];

                using var client = new SmtpClient();

                await client.ConnectAsync(host, port, options, cancellationToken);

                await client.AuthenticateAsync(user, pwd, cancellationToken);

                await client.SendAsync(FormatOptions.Default, mimeMessage, cancellationToken);

                await client.DisconnectAsync(true, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not send mail");

                if (cTry == 5)
                    throw;

                await TryInternalSendAsync(mimeMessage, cTry + 1, cancellationToken);
            }
        }
    }
}