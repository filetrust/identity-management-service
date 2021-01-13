using System;
using System.Collections.Generic;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Models.Email;
using Glasswall.IdentityManagementService.Common.Models.Store;

namespace Glasswall.IdentityManagementService.Api.Controllers
{
    public class ResetPasswordEmail : EmailModel
    {
        private readonly User _createdUser;
        private readonly IIdentityManagementServiceConfiguration _config;
        private readonly string _resetToken;

        public ResetPasswordEmail(User createdUser, IIdentityManagementServiceConfiguration config, string resetToken)
        {
            _createdUser = createdUser ?? throw new System.ArgumentNullException(nameof(createdUser));
            _config = config ?? throw new System.ArgumentNullException(nameof(config));
            _resetToken = resetToken ?? throw new ArgumentNullException(nameof(resetToken));
        }

        public override string Body => $"Please reset your password at '{_config.ManagementUIEndpoint}/reset?Token={_resetToken}";
        public override string Subject => "New user notification";
        public override string EmailFrom => "Glasswall";
        public override IEnumerable<string> EmailTo => new[] { _createdUser.Email };
    }
}