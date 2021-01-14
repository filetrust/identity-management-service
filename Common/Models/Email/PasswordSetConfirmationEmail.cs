using System;
using System.Collections.Generic;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Models.Store;

namespace Glasswall.IdentityManagementService.Common.Models.Email
{
    public class PasswordSetConfirmationEmail : EmailModel
    {
        private readonly User _createdUser;
        private readonly IIdentityManagementServiceConfiguration _config;

        public PasswordSetConfirmationEmail(User createdUser, IIdentityManagementServiceConfiguration config)
        {
            _createdUser = createdUser ?? throw new ArgumentNullException(nameof(createdUser));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public override string Body => $"You have successfully set your password. Please log into your account <a href=\"{_config.ManagementUIEndpoint}\">here</a>.";
        public override string Subject => "Your Password has been changed.";
        public override string EmailFrom => "admin@glasswallsolutions.com";
        public override IEnumerable<string> EmailTo => new[] { _createdUser.Email };
    }
}