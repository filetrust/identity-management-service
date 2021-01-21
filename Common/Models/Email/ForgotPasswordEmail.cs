using System;
using System.Collections.Generic;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Models.Store;

namespace Glasswall.IdentityManagementService.Common.Models.Email
{
    public class ForgotPasswordEmail : EmailModel
    {
        private readonly User _createdUser;
        private readonly IIdentityManagementServiceConfiguration _config;
        private readonly string _resetToken;

        public ForgotPasswordEmail(User createdUser, IIdentityManagementServiceConfiguration config, string resetToken)
        {
            _createdUser = createdUser ?? throw new ArgumentNullException(nameof(createdUser));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _resetToken = resetToken ?? throw new ArgumentNullException(nameof(resetToken));
        }

        public override string Body =>
            $"Please reset your password at '{_config.ManagementUIEndpoint}/reset?Token={_resetToken}";

        public override string Subject => "Password reset notification";
        public override string EmailFrom => "admin@glasswallsolutions.com";
        public override IEnumerable<string> EmailTo => new[] {_createdUser.Email};
    }
}